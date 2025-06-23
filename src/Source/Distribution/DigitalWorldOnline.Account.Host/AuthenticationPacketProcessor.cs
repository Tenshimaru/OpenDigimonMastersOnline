using AutoMapper;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Commands.Delete;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Models.Servers;
using DigitalWorldOnline.Commons.Packets.AuthenticationServer;
using DigitalWorldOnline.Account.Models.Configuration;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text;
using DigitalWorldOnline.Application.Admin.Commands;
using Microsoft.Extensions.Options;
using System.Security;

namespace DigitalWorldOnline.Account
{
    public sealed class AuthenticationPacketProcessor : IProcessor, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly ILogger _logger;
        private readonly AuthenticationServerConfigurationModel _authenticationServerConfiguration;

        private const string CharacterServerAddress = "CharacterServer:Address";

        private const int HandshakeDegree = 32321;

        public AuthenticationPacketProcessor(IMapper mapper, ILogger logger, ISender sender,
            IConfiguration configuration,
            IOptions<AuthenticationServerConfigurationModel> authenticationServerConfiguration)
        {
            _configuration = configuration;
            _authenticationServerConfiguration = authenticationServerConfiguration.Value;
            _mapper = mapper;
            _sender = sender;
            _logger = logger;
        }

        /// <summary>
        /// Process the arrived TCP packet, sent from the game client
        /// </summary>
        /// <param name="client">The game client whos sended the packet</param>
        /// <param name="data">The packet bytes array</param>
        public async Task ProcessPacketAsync(GameClient client, byte[] data)
        {
            var packet = new AuthenticationPacketReader(data);

            _logger.Debug("Received packet type {Type} from {Address}", packet.Enum, client.ClientAddress);
            switch (packet.Enum)
            {
                case AuthenticationServerPacketEnum.Connection:
                {
                    var kind = packet.ReadByte();

                    var handshakeTimestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    var handshake = (short)(client.Handshake ^ HandshakeDegree);

                    client.Send(new ConnectionPacket(handshake, handshakeTimestamp));
                }
                    break;

                case AuthenticationServerPacketEnum.KeepConnection:
                    break;

                case AuthenticationServerPacketEnum.LoginRequest:
                {
                    var username = ExtractUsername(packet);
                    var password = ExtractPassword(packet, username);
                    var cpu = ExtractCpu(packet, username, password);
                    var gpu = ExtractGpu(packet, username, password, cpu);

                    _logger.Debug("Validating login data for {Username}", username);
                    var account = await _sender.Send(new AccountByUsernameQuery(username));

                    if (account == null)
                    {
                        _logger.Debug("Saving {Username} login try for incorrect username...", username);

                        await _sender.Send(new CreateLoginTryCommand(username, client.ClientAddress, LoginTryResultEnum.IncorrectUsername));

                        client.Send(new LoginRequestAnswerPacket(LoginFailReasonEnum.UserNotFound));

                        break;
                    }

                    client.SetAccountId(account.Id);
                    client.SetAccessLevel(account.AccessLevel);

                    if (account.AccountBlock != null)
                    {
                        var blockInfo =
                            _mapper.Map<AccountBlockModel>(
                                await _sender.Send(new AccountBlockByIdQuery(account.AccountBlock.Id)));

                        if (blockInfo.EndDate > DateTime.Now)
                        {
                            TimeSpan timeRemaining = blockInfo.EndDate - DateTime.Now;

                            uint secondsRemaining = (uint)timeRemaining.TotalSeconds;
                            _logger.Debug($"Saving {username} login try for blocked account...");

                            await _sender.Send(new CreateLoginTryCommand(username, client.ClientAddress,
                                LoginTryResultEnum.AccountBlocked));
                            client.Send(new LoginRequestBannedAnswerPacket(secondsRemaining, blockInfo.Reason));
                            break;
                        }
                        else
                        {
                            await _sender.Send(new DeleteBanCommand(blockInfo.Id));
                        }
                    }

                    if (account.Password != password.Encrypt())
                    {
                        DebugLog($"Saving {username} login try for incorrect password...");
                        await _sender.Send(new CreateLoginTryCommand(username, client.ClientAddress,
                            LoginTryResultEnum.IncorrectPassword));

                        client.Send(new LoginRequestAnswerPacket(LoginFailReasonEnum.IncorrectPassword));
                        break;
                    }

                    /*client.Send(account.SecondaryPassword == null
                        ? new LoginRequestAnswerPacket(SecondaryPasswordScreenEnum.RequestSetup)
                        : new LoginRequestAnswerPacket(SecondaryPasswordScreenEnum.RequestInput));*/

                    client.Send(new LoginRequestAnswerPacket(SecondaryPasswordScreenEnum.Hide));

                    if (_authenticationServerConfiguration.UseHash)
                    {
                        _logger.Debug("Getting resources hash from database !!");
                        var hashString = await _sender.Send(new ResourcesHashQuery());

                        _logger.Debug("Sending Hash to client");
                        client.Send(new ResourcesHashPacket(hashString));
                    }

                    if (account.SystemInformation == null)
                    {
                        DebugLog($"Creating system information...");
                        await _sender.Send(
                            new CreateSystemInformationCommand(account.Id, cpu, gpu, client.ClientAddress));
                    }
                    else
                    {
                        DebugLog($"Updating system information...");
                        await _sender.Send(new UpdateSystemInformationCommand(account.SystemInformation.Id, account.Id,
                            cpu, gpu, client.ClientAddress));
                    }
                }
                    break;

                case AuthenticationServerPacketEnum.SecondaryPasswordRegister:
                {
                    DebugLog("Reading packet parameters...");
                    var securityPassword = packet.ReadZString();

                    DebugLog($"Updating {client.AccountId} account information...");
                    await _sender.Send(new CreateOrUpdateSecondaryPasswordCommand(client.AccountId, securityPassword));

                    client.Send(new LoginRequestAnswerPacket(SecondaryPasswordScreenEnum.RequestInput));
                }
                    break;

                case AuthenticationServerPacketEnum.SecondaryPasswordCheck:
                {
                    DebugLog("Reading packet first part parameters...");
                    var needToCheck = packet.ReadShort() == SecondaryPasswordCheckEnum.Check.GetHashCode();

                    DebugLog($"Searching account with id {client.AccountId}...");
                    var account = await _sender.Send(new AccountByIdQuery(client.AccountId));

                    if (account == null)
                        throw new KeyNotFoundException(nameof(account));

                    if (needToCheck)
                    {
                        DebugLog("Reading packet second part parameters...");
                        var securityCode = packet.ReadZString();

                        if (account.SecondaryPassword == securityCode)
                        {
                            _logger.Debug("Saving login try for skipping secondary password...");
                            await _sender.Send(new CreateLoginTryCommand(account.Username, client.ClientAddress,
                                LoginTryResultEnum.Success));
                            client.Send(
                                new SecondaryPasswordCheckResultPacket(SecondaryPasswordCheckEnum.CorrectOrSkipped));
                        }
                        else
                        {
                            _logger.Debug("Saving login try for incorrect secondary password...");
                            await _sender.Send(new CreateLoginTryCommand(account.Username, client.ClientAddress,
                                LoginTryResultEnum.IncorrectSecondaryPassword));
                            client.Send(new SecondaryPasswordCheckResultPacket(SecondaryPasswordCheckEnum.Incorrect));
                        }
                    }
                    else
                    {
                        DebugLog("Saving login try for skipping secondary password...");
                        await _sender.Send(new CreateLoginTryCommand(account.Username, client.ClientAddress,
                            LoginTryResultEnum.Success));

                        DebugLog($"Sending answer for skipped secondary password check...");
                        client.Send(new SecondaryPasswordCheckResultPacket(SecondaryPasswordCheckEnum.CorrectOrSkipped)
                            .Serialize());
                    }
                }
                    break;

                case AuthenticationServerPacketEnum.SecondaryPasswordChange:
                {
                    DebugLog("Getting packet parameters...");
                    var currentSecurityCode = packet.ReadZString();
                    var newSecurityCode = packet.ReadZString();

                    var account = await _sender.Send(new AccountByIdQuery(client.AccountId));

                    if (account == null)
                        throw new KeyNotFoundException(nameof(account));

                    if (account.SecondaryPassword == currentSecurityCode)
                    {
                        DebugLog($"Saving new secondary password...");
                        await _sender.Send(
                            new CreateOrUpdateSecondaryPasswordCommand(client.AccountId, newSecurityCode));

                        client.Send(new SecondaryPasswordChangeResultPacket(SecondaryPasswordChangeEnum.Changed)
                            .Serialize());
                    }
                    else
                    {
                        DebugLog($"Sending answer for incorrect secondary password change...");
                        client.Send(
                            new SecondaryPasswordChangeResultPacket(SecondaryPasswordChangeEnum.IncorretCurrentPassword)
                                .Serialize());
                    }
                }
                    break;

                case AuthenticationServerPacketEnum.LoadServerList:
                {
                    DebugLog($"Getting server list...");
                    var servers =
                        _mapper.Map<IEnumerable<ServerObject>>(
                            await _sender.Send(new ServersQuery(client.AccessLevel)));

                    var serverObjects = servers.ToList();
                    foreach (var server in serverObjects)
                    {
                        server.UpdateCharacterCount(
                            await _sender.Send(new CharactersInServerQuery(client.AccountId, server.Id)));
                    }

                    DebugLog($"Sending server list...");
                    client.Send(new ServerListPacket(serverObjects).Serialize());
                }
                    break;

                case AuthenticationServerPacketEnum.ConnectCharacterServer:
                {
                    DebugLog($"Reading packet parameters...");
                    var serverId = packet.ReadInt();

                    await _sender.Send(new UpdateLastPlayedServerCommand(client.AccountId, serverId));

                    if (_authenticationServerConfiguration.UseHash)
                    {
                        _logger.Debug("Getting resources hash.");
                        var hashString = await _sender.Send(new ResourcesHashQuery());

                        client.Send(new ResourcesHashPacket(hashString));
                    }

                    DebugLog($"Getting server list...");
                    var servers =
                        _mapper.Map<IEnumerable<ServerObject>>(
                            await _sender.Send(new ServersQuery(client.AccessLevel)));

                    var targetServer = servers.First(x => x.Id == serverId);

                    DebugLog($"Sending selected server info...");
                    client.Send(new ConnectCharacterServerPacket(client.AccountId,
                        _configuration[CharacterServerAddress], targetServer.Port.ToString()));
                }
                    break;

                case AuthenticationServerPacketEnum.Unknown:
                case AuthenticationServerPacketEnum.ResourcesHash:
                    {
                        _logger.Information($"Verifying Hash -> Packet 10003");

                        int hashLength = BitConverter.ToInt16(data, 0);
                        string clientHash = BitConverter.ToString(data, 2, hashLength).Replace("-", "");

                        _logger.Information($"Hash received:\n{clientHash}");
                    }
                    break;

                default:
                {
                    _logger.Warning($"Unknown packet. Type: {packet.Type} Length: {packet.Length}.");
                }
                    break;
            }
        }

        private static string ExtractGpu(AuthenticationPacketReader packet, string username, string password,
            string cpu)
        {
            packet.Seek(9 + username.Length + 2 + password.Length + 2 + cpu.Length + 2);

            var gpuSize = packet.ReadByte();

            var gpuArray = new byte[gpuSize];

            for (int i = 0; i < gpuSize; i++)
                gpuArray[i] = packet.ReadByte();

            return Encoding.ASCII.GetString(gpuArray).Trim();
        }

        private static string ExtractCpu(AuthenticationPacketReader packet, string username, string password)
        {
            packet.Seek(9 + username.Length + 2 + password.Length + 2);

            var cpuSize = packet.ReadByte();

            var cpuArray = new byte[cpuSize];

            for (int i = 0; i < cpuSize; i++)
                cpuArray[i] = packet.ReadByte();

            return Encoding.ASCII.GetString(cpuArray).Trim();
        }

        private static string ExtractPassword(AuthenticationPacketReader packet, string username)
        {
            packet.Seek(9 + username.Length + 2);
            var passwordSize = packet.ReadByte();

            var passwordArray = new byte[passwordSize];

            for (int i = 0; i < passwordSize; i++)
                passwordArray[i] = packet.ReadByte();

            return Encoding.ASCII.GetString(passwordArray).Trim();
        }

        private static string ExtractUsername(AuthenticationPacketReader packet)
        {
            packet.Seek(9);
            var usernameSize = packet.ReadByte();
            var usernameArray = new byte[usernameSize];

            for (int i = 0; i < usernameSize; i++)
                usernameArray[i] = packet.ReadByte();

            return Encoding.ASCII.GetString(usernameArray).Trim();
        }

        /// <summary>
        /// Shortcut for debug logging with client and packet info.
        /// </summary>
        /// <param name="message">The message to log</param>
        private void DebugLog(string message)
        {
            _logger?.Debug($"{message}");
        }

        /// <summary>
        /// Disposes the entire object.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
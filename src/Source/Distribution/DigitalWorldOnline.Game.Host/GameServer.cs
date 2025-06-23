using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.AuthenticationServer;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DigitalWorldOnline.Game
{
    public sealed class GameServer : Commons.Entities.GameServer, IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _configuration;
        private readonly IProcessor _processor;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly PvpServer _pvpServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly EventServer _eventServer;
        private readonly PartyManager _partyManager;

        private const int OnConnectEventHandshakeHandler = 65535;

        public GameServer(
            IHostApplicationLifetime hostApplicationLifetime,
            IConfiguration configuration,
            IProcessor processor,
            ILogger logger,
            IMapper mapper,
            ISender sender,
            AssetsLoader assets,
            MapServer mapServer,
            PvpServer pvpServer,
            DungeonsServer dungeonsServer,
            EventServer eventServer,
            PartyManager partyManager)
        {
            OnConnect += OnConnectEvent;
            OnDisconnect += OnDisconnectEvent;
            DataReceived += OnDataReceivedEvent;

            _hostApplicationLifetime = hostApplicationLifetime;
            _configuration = configuration;
            _processor = processor;
            _logger = logger;
            _mapper = mapper;
            _sender = sender;
            _assets = assets;
            _mapServer = mapServer;
            _pvpServer = pvpServer;
            _dungeonsServer = dungeonsServer;
            _eventServer = eventServer;
            _partyManager = partyManager;
        }

        /// <summary>
        /// Event triggered everytime that a game client connects to the server.
        /// </summary>
        /// <param name="sender">The object itself</param>
        /// <param name="gameClientEvent">Game client who connected</param>
        private void OnConnectEvent(object sender, GameClientEvent gameClientEvent)
        {
            var clientIpAddress = gameClientEvent.Client.ClientAddress.Split(':')?.FirstOrDefault();

            /*if (InvalidConnection(clientIpAddress))
            {
                _logger.Warning($"Blocked connection event from {gameClientEvent.Client.HiddenAddress}.");

                if (!string.IsNullOrEmpty(clientIpAddress) && !RefusedAddresses.Contains(clientIpAddress))
                    RefusedAddresses.Add(clientIpAddress);

                gameClientEvent.Client.Disconnect();
                RemoveClient(gameClientEvent.Client);
            }*/

            _logger.Information($"Accepted connection event from {gameClientEvent.Client.HiddenAddress}.");

            gameClientEvent.Client.SetHandshake((short)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() &
                                                        OnConnectEventHandshakeHandler));

            if (gameClientEvent.Client.IsConnected)
            {
                _logger.Debug($"Sending handshake for request source {gameClientEvent.Client.ClientAddress}.");
                gameClientEvent.Client.Send(new OnConnectEventConnectionPacket(gameClientEvent.Client.Handshake));
            }
            else
                _logger.Warning($"Request source {gameClientEvent.Client.ClientAddress} has been disconnected.");
        }

        /// <summary>
        /// Event triggered everytime the game client disconnects from the server.
        /// </summary>
        /// <param name="sender">The object itself</param>
        /// <param name="gameClientEvent">Game client who disconnected</param>
        private async void OnDisconnectEvent(object sender, GameClientEvent gameClientEvent)
        {
            if (gameClientEvent.Client.TamerId > 0)
            {
                _logger.Information(
                    $"Received disconnection event for {gameClientEvent.Client.Tamer.Name} {gameClientEvent.Client.TamerId} {gameClientEvent.Client.HiddenAddress}.");

                _logger.Debug(
                    $"Source disconnected: {gameClientEvent.Client.ClientAddress}. Account: {gameClientEvent.Client.AccountId}.");

                if (gameClientEvent.Client.DungeonMap)
                {
                    _logger.Information(
                        $"Removing the tamer {gameClientEvent.Client.Tamer.Name} . {gameClientEvent.Client.HiddenAddress}.");
                    _dungeonsServer.RemoveClient(gameClientEvent.Client);
                }
                else if (gameClientEvent.Client.EventMap)
                {
                    _logger.Information(
                        $"Removing the tamer {gameClientEvent.Client.Tamer.Name} . {gameClientEvent.Client.HiddenAddress}.");
                    _eventServer.RemoveClient(gameClientEvent.Client);
                }
                else if (gameClientEvent.Client.PvpMap)
                {
                    _logger.Information(
                        $"Removing the tamer {gameClientEvent.Client.Tamer.Name} . {gameClientEvent.Client.HiddenAddress}.");
                    _pvpServer.RemoveClient(gameClientEvent.Client);
                }
                else
                {
                    _logger.Information(
                        $"Removing the tamer {gameClientEvent.Client.Tamer.Name} {gameClientEvent.Client.TamerId}. {gameClientEvent.Client.HiddenAddress}.");
                    _mapServer.RemoveClient(gameClientEvent.Client);
                }

                if (gameClientEvent.Client.GameQuit)
                {
                    gameClientEvent.Client.Tamer.UpdateState(CharacterStateEnum.Disconnected);
                    _logger.Information(
                        $"Updating character {gameClientEvent.Client.Tamer.Name} {gameClientEvent.Client.TamerId} state upon disconnect...");
                    await _sender.Send(new UpdateCharacterStateCommand(gameClientEvent.Client.TamerId,
                        CharacterStateEnum.Disconnected));

                    CharacterFriendsNotification(gameClientEvent);
                    CharacterGuildNotification(gameClientEvent);
                    await PartyNotification(gameClientEvent);
                    CharacterTargetTraderNotification(gameClientEvent);

                    if (gameClientEvent.Client.DungeonMap)
                    {
                        await DungeonWarpGate(gameClientEvent);
                    }
                }
            }
        }

        private async Task PartyNotification(GameClientEvent gameClientEvent)
        {
            var party = _partyManager.FindParty(gameClientEvent.Client.TamerId);

            if (party != null)
            {
                var member = party.Members.FirstOrDefault(x => x.Value.Id == gameClientEvent.Client.TamerId);

                foreach (var target in party.Members.Values)
                {
                    var targetClient = _mapServer.FindClientByTamerId(target.Id);

                    if (targetClient == null) targetClient = _dungeonsServer.FindClientByTamerId(target.Id);

                    if (targetClient == null) continue;

                    targetClient.Send(new PartyMemberDisconnectedPacket(party[gameClientEvent.Client.TamerId].Key)
                        .Serialize());
                }

                if (member.Key == party.LeaderId && party.Members.Count >= 3)
                {
                    party.RemoveMember(party[gameClientEvent.Client.TamerId].Key);

                    var randomIndex = new Random().Next(party.Members.Count);
                    var sortedPlayer = party.Members.ElementAt(randomIndex).Key;

                    foreach (var target in party.Members.Values)
                    {
                        var targetClient = _mapServer.FindClientByTamerId(target.Id);

                        if (targetClient == null) targetClient = _dungeonsServer.FindClientByTamerId(target.Id);

                        if (targetClient == null) continue;

                        targetClient.Send(new PartyLeaderChangedPacket(sortedPlayer).Serialize());
                    }
                }
                else
                {
                    if (party.Members.Count == 2)
                    {
                        var map = UtilitiesFunctions.MapGroup(gameClientEvent.Client.Tamer.Location.MapId);

                        var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(map));
                        var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(map));

                        if (mapConfig == null || waypoints == null || !waypoints.Regions.Any())
                        {
                            gameClientEvent.Client.Send(
                                new SystemMessagePacket($"Map information not found for map Id {map}."));
                            _logger.Warning(
                                $"Map information not found for map Id {map} on character {gameClientEvent.Client.TamerId}.");
                            _partyManager.RemoveParty(party.Id);
                            return;
                        }

                        var destination = waypoints.Regions.First();

                        foreach (var pmember in party.Members.Values.Where(x => x.Id != gameClientEvent.Client.Tamer.Id)
                                     .ToList())
                        {
                            var dungeonClient = _dungeonsServer.FindClientByTamerId(pmember.Id);

                            if (dungeonClient == null) continue;

                            if (dungeonClient.DungeonMap)
                            {
                                _dungeonsServer.RemoveClient(dungeonClient);

                                dungeonClient.Tamer.NewLocation(map, destination.X, destination.Y);
                                await _sender.Send(new UpdateCharacterLocationCommand(dungeonClient.Tamer.Location));

                                dungeonClient.Tamer.Partner.NewLocation(map, destination.X, destination.Y);
                                await _sender.Send(
                                    new UpdateDigimonLocationCommand(dungeonClient.Tamer.Partner.Location));

                                dungeonClient.Tamer.UpdateState(CharacterStateEnum.Loading);
                                await _sender.Send(new UpdateCharacterStateCommand(dungeonClient.TamerId,
                                    CharacterStateEnum.Loading));

                                foreach (var memberId in party.GetMembersIdList())
                                {
                                    var targetDungeon = _dungeonsServer.FindClientByTamerId(memberId);
                                    if (targetDungeon != null)
                                        targetDungeon.Send(new PartyMemberWarpGatePacket(party[dungeonClient.TamerId],
                                                gameClientEvent.Client.Tamer)
                                            .Serialize());
                                }

                                dungeonClient?.SetGameQuit(false);

                                dungeonClient?.Send(new MapSwapPacket(_configuration[GamerServerPublic],
                                    _configuration[GameServerPort],
                                    dungeonClient.Tamer.Location.MapId, dungeonClient.Tamer.Location.X,
                                    dungeonClient.Tamer.Location.Y));
                            }
                        }
                    }

                    party.RemoveMember(party[gameClientEvent.Client.TamerId].Key);
                }

                if (party.Members.Count <= 1)
                    _partyManager.RemoveParty(party.Id);
            }
        }

        private void CharacterGuildNotification(GameClientEvent gameClientEvent)
        {
            if (gameClientEvent.Client.Tamer.Guild != null)
            {
                foreach (var guildMember in gameClientEvent.Client.Tamer.Guild.Members)
                {
                    if (guildMember.CharacterInfo == null)
                    {
                        var guildMemberClient = _mapServer.FindClientByTamerId(guildMember.CharacterId);

                        if (guildMemberClient != null)
                        {
                            guildMember.SetCharacterInfo(guildMemberClient.Tamer);
                        }
                        else
                        {
                            guildMember.SetCharacterInfo(_mapper.Map<CharacterModel>(_sender
                                .Send(new CharacterByIdQuery(guildMember.CharacterId)).Result));
                        }
                    }
                }

                foreach (var guildMember in gameClientEvent.Client.Tamer.Guild.Members)
                {
                    _logger.Debug(
                        $"Sending guild member disconnection packet for character {guildMember.CharacterId}...");

                    _logger.Debug(
                        $"Sending guild information packet for character {gameClientEvent.Client.TamerId}...");

                    _mapServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                        new GuildMemberDisconnectPacket(gameClientEvent.Client.Tamer.Name).Serialize());

                    _mapServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                        new GuildInformationPacket(gameClientEvent.Client.Tamer.Guild).Serialize());

                    _dungeonsServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                        new GuildMemberDisconnectPacket(gameClientEvent.Client.Tamer.Name).Serialize());

                    _dungeonsServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                        new GuildInformationPacket(gameClientEvent.Client.Tamer.Guild).Serialize());

                    _eventServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                        new GuildMemberDisconnectPacket(gameClientEvent.Client.Tamer.Name).Serialize());

                    _eventServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                        new GuildInformationPacket(gameClientEvent.Client.Tamer.Guild).Serialize());

                    _pvpServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                        new GuildMemberDisconnectPacket(gameClientEvent.Client.Tamer.Name).Serialize());

                    _pvpServer.BroadcastForUniqueTamer(guildMember.CharacterId,
                        new GuildInformationPacket(gameClientEvent.Client.Tamer.Guild).Serialize());
                }
            }
        }

        private async void CharacterFriendsNotification(GameClientEvent gameClientEvent)
        {
            gameClientEvent.Client.Tamer.Friended.ForEach(friend =>
            {
                _logger.Information($"Sending friend disconnection packet for character {friend.FriendId}...");
                _mapServer.BroadcastForUniqueTamer(friend.FriendId,
                    new FriendDisconnectPacket(gameClientEvent.Client.Tamer.Name).Serialize());
                _dungeonsServer.BroadcastForUniqueTamer(friend.FriendId,
                    new FriendDisconnectPacket(gameClientEvent.Client.Tamer.Name).Serialize());
                _eventServer.BroadcastForUniqueTamer(friend.FriendId,
                    new FriendDisconnectPacket(gameClientEvent.Client.Tamer.Name).Serialize());
                _pvpServer.BroadcastForUniqueTamer(friend.FriendId,
                    new FriendDisconnectPacket(gameClientEvent.Client.Tamer.Name).Serialize());
            });

            await _sender.Send(new UpdateCharacterFriendsCommand(gameClientEvent.Client.Tamer, false));
        }

        private void CharacterTargetTraderNotification(GameClientEvent gameClientEvent)
        {
            if (gameClientEvent.Client.Tamer.TargetTradeGeneralHandle != 0)
            {
                if (gameClientEvent.Client.DungeonMap)
                {
                    var targetClient =
                        _dungeonsServer.FindClientByTamerHandle(gameClientEvent.Client.Tamer.TargetTradeGeneralHandle);

                    if (targetClient != null)
                    {
                        targetClient.Send(new TradeCancelPacket(gameClientEvent.Client.Tamer.GeneralHandler));
                        targetClient.Tamer.ClearTrade();
                    }
                }
                else
                {
                    var targetClient = _mapServer.FindClientByTamerHandleAndChannel(
                        gameClientEvent.Client.Tamer.TargetTradeGeneralHandle, gameClientEvent.Client.TamerId);

                    if (targetClient != null)
                    {
                        targetClient.Send(new TradeCancelPacket(gameClientEvent.Client.Tamer.GeneralHandler));
                        targetClient.Tamer.ClearTrade();
                    }
                }
            }
        }

        private async Task DungeonWarpGate(GameClientEvent gameClientEvent)
        {
            if (gameClientEvent.Client.DungeonMap)
            {
                var map = UtilitiesFunctions.MapGroup(gameClientEvent.Client.Tamer.Location.MapId);

                var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(map));
                var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(map));

                if (mapConfig == null || waypoints == null || !waypoints.Regions.Any())
                {
                    gameClientEvent.Client.Send(
                        new SystemMessagePacket($"Map information not found for map Id {map}."));
                    _logger.Warning(
                        $"Map information not found for map Id {map} on character {gameClientEvent.Client.TamerId} Dungeon Portal");
                    return;
                }

                var destination = waypoints.Regions.First();

                gameClientEvent.Client.Tamer.NewLocation(map, destination.X, destination.Y);
                await _sender.Send(new UpdateCharacterLocationCommand(gameClientEvent.Client.Tamer.Location));

                gameClientEvent.Client.Tamer.Partner.NewLocation(map, destination.X, destination.Y);
                await _sender.Send(new UpdateDigimonLocationCommand(gameClientEvent.Client.Tamer.Partner.Location));

                gameClientEvent.Client.Tamer.UpdateState(CharacterStateEnum.Loading);
                await _sender.Send(new UpdateCharacterStateCommand(gameClientEvent.Client.TamerId,
                    CharacterStateEnum.Loading));
            }
        }

        /// <summary>
        /// Event triggered everytime the game client sends a TCP packet.
        /// </summary>
        /// <param name="sender">The object itself</param>
        /// <param name="gameClientEvent">Game client who sent the packet</param>
        /// <param name="data">The packet content, in byte array</param>
        private void OnDataReceivedEvent(object sender, GameClientEvent gameClientEvent, byte[] data)
        {
            try
            {
                _logger.Debug($"Received {data.Length} bytes from {gameClientEvent.Client.ClientAddress}.");
                _processor.ProcessPacketAsync(gameClientEvent.Client, data);
            }
            catch (NotImplementedException)
            {
                gameClientEvent.Client.Send(new SystemMessagePacket($"Feature under development."));
            }
            catch (Exception ex)
            {
                gameClientEvent.Client.SetGameQuit(true);
                gameClientEvent.Client.Disconnect();

                _logger.Error($"Process packet error: {ex.Message} {ex.InnerException} {ex.StackTrace}.");

                try
                {
                    var filePath = $"PacketErrors/{gameClientEvent.Client.ClientAddress}_{DateTime.Now}.txt";

                    using var fs = File.Create(filePath);
                    fs.Write(data, 0, data.Length);
                }
                catch
                {
                }

                //TODO: Salvar no banco com os parametros
            }
        }

        /// <summary>
        /// The default hosted service "starting" method.
        /// </summary>
        /// <param name="cancellationToken">Control token for the operation</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"Starting {GetType().Name}...");

            Console.Title = $"DMO - {GetType().Name}";

            _hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
            _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
            _hostApplicationLifetime.ApplicationStopped.Register(OnStopped);

            Task.Run(CheckAllDigimonEvolutions);
            Task.Run(() => _mapServer.StartAsync(cancellationToken));
            Task.Run(() => _mapServer.LoadAllMaps(cancellationToken));
            //Task.Run(() => _mapServer.CallDiscordWarnings("Server Online", "13ff00", "1307467492888805476", "1280948869739450438"));
            Task.Run(() => _dungeonsServer.StartAsync(cancellationToken));
            Task.Run(() => _pvpServer.StartAsync(cancellationToken));
            Task.Run(() => _eventServer.StartAsync(cancellationToken));

            Task.Run(() => _sender.Send(new UpdateCharacterFriendsCommand(null, false)));

            return Task.CompletedTask;
        }

        /// <summary>
        /// The default hosted service "stopping" method
        /// </summary>
        /// <param name="cancellationToken">Control token for the operation</param>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// The default hosted service "started" method action
        /// </summary>
        private void OnStarted()
        {
            if (!Listen(_configuration[GameServerAddress], _configuration[GameServerPort],
                    _configuration[GameServerBacklog]))
            {
                _logger.Error("Unable to start. Check the binding configurations.");
                _hostApplicationLifetime.StopApplication();
                return;
            }

            _logger.Information($"{GetType().Name} started.");

            _sender.Send(new UpdateCharactersStateCommand(CharacterStateEnum.Disconnected));
        }

        /// <summary>
        /// The default hosted service "stopping" method action
        /// </summary>
        private void OnStopping()
        {
            try
            {
                _logger.Information($"Disconnecting clients from {GetType().Name}...");

                Task.Run(async () => await _sender.Send(new UpdateCharacterFriendsCommand(null, false)));

                _logger.Information($"Made all friends offline {GetType().Name}...");

                //_ = _mapServer.CallDiscordWarnings("Server Offline", "fc0303", "1307467492888805476", "1280948869739450438");
                Shutdown();
                return;
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }

        /// <summary>
        /// The default hosted service "stopped" method action
        /// </summary>
        private void OnStopped()
        {
            _logger.Information($"{GetType().Name} stopped.");
        }

        private async Task<Task> CheckAllDigimonEvolutions()
        {
            List<DigimonModel> Digimons =
                _mapper.Map<List<DigimonModel>>(await _sender.Send(new GetAllCharactersDigimonQuery()));

            int digimonCount = 0;
            int encyclopediaCount = 0;
            int encyclopediaEvolutionCount = 0;
            Digimons.ForEach(async void (digimon) =>
            {
                try
                {
                    var digimonEvolutionInfo =
                        _mapper.Map<EvolutionAssetModel>(
                            await _sender.Send(new DigimonEvolutionAssetsByTypeQuery(digimon.BaseType)));
                    if (digimonEvolutionInfo == null)
                    {
                        _logger.Warning($"EvolutionInfo is null for digimon {digimon.BaseType}.");
                        return;
                    }

                    if (digimonEvolutionInfo != null && digimon.Character.Encyclopedia != null)
                    {
                        var encyclopediaExists =
                            digimon.Character.Encyclopedia.Exists(x => x.DigimonEvolutionId == digimonEvolutionInfo.Id);

                        foreach (var evolutionLine in digimonEvolutionInfo.Lines)
                        {
                            if (!digimon.Evolutions.Exists(x => x.Type == evolutionLine.Type))
                            {
                                digimonCount++;
                                digimon.Evolutions.Add(new DigimonEvolutionModel(evolutionLine.Type));
                            }
                        }

                        // Check if encyclopedia exists
                        if (!encyclopediaExists)
                        {
                            encyclopediaCount++;
                            var encyclopedia = CharacterEncyclopediaModel.Create(digimon.Character.Id,
                                digimonEvolutionInfo.Id, digimon.Level, digimon.Size, digimon.Digiclone.ATLevel,
                                digimon.Digiclone.BLLevel, digimon.Digiclone.CTLevel, digimon.Digiclone.EVLevel,
                                digimon.Digiclone.HPLevel,
                                digimon.Evolutions.Count(x => Convert.ToBoolean(x.Unlocked)) ==
                                digimon.Evolutions.Count,
                                false);

                            digimon.Evolutions?.ForEach(x =>
                            {
                                encyclopediaEvolutionCount++;
                                var evolutionLine = digimonEvolutionInfo.Lines.FirstOrDefault(y => y.Type == x.Type);
                                byte slotLevel = 0;

                                if (evolutionLine != null)
                                {
                                    slotLevel = evolutionLine.SlotLevel;
                                }

                                var encyclopediaEvo =
                                    CharacterEncyclopediaEvolutionsModel.Create(x.Type, slotLevel,
                                        Convert.ToBoolean(x.Unlocked));
                                _logger.Debug(
                                    $"{encyclopediaEvo.Id}, {encyclopediaEvo.DigimonBaseType}, {encyclopediaEvo.SlotLevel}, {encyclopediaEvo.IsUnlocked}");

                                encyclopedia.Evolutions.Add(encyclopediaEvo);
                            });


                            var encyclopediaAdded =
                                await _sender.Send(new CreateCharacterEncyclopediaCommand(encyclopedia));
                            digimon.Character.Encyclopedia.Add(encyclopediaAdded);
                        }
                        else
                        {
                            digimon?.Evolutions?.ForEach(async void (evolution) =>
                            {
                                try
                                {
                                    var evolutionLine =
                                        digimonEvolutionInfo.Lines.FirstOrDefault(y => y.Type == evolution.Type);
                                    byte slotLevel = 0;

                                    if (evolutionLine != null)
                                    {
                                        slotLevel = evolutionLine.SlotLevel;
                                    }

                                    if (!digimon.Character.Encyclopedia.Exists(x =>
                                            x.DigimonEvolutionId == digimonEvolutionInfo?.Id &&
                                            x.Evolutions.Exists(evo => evo.DigimonBaseType == evolution.Type)))
                                    {
                                        encyclopediaEvolutionCount++;
                                        var encyclopediaEvo =
                                            CharacterEncyclopediaEvolutionsModel.Create(evolution.Type, slotLevel,
                                                Convert.ToBoolean(evolution.Unlocked));

                                        _logger.Debug(
                                            $"{encyclopediaEvo.Id}, {encyclopediaEvo.DigimonBaseType}, {encyclopediaEvo.SlotLevel}, {encyclopediaEvo.IsUnlocked}");

                                        digimon.Character.Encyclopedia
                                            .First(x => x.DigimonEvolutionId == digimonEvolutionInfo?.Id)
                                            ?.Evolutions.Add(encyclopediaEvo);

                                        var lockedEncyclopediaCount = digimon.Character.Encyclopedia
                                            .First(x => x.DigimonEvolutionId == digimonEvolutionInfo?.Id)
                                            .Evolutions.Count(x => x.IsUnlocked == false);

                                        if (lockedEncyclopediaCount <= 0)
                                        {
                                            digimon.Character.Encyclopedia
                                                .First(x => x.DigimonEvolutionId == digimonEvolutionInfo?.Id)
                                                .SetRewardAllowed();
                                            digimon.Character.Encyclopedia
                                                .First(x => x.DigimonEvolutionId == digimonEvolutionInfo?.Id)
                                                .SetRewardReceived(false);
                                            await _sender.Send(new UpdateCharacterEncyclopediaCommand(
                                                digimon.Character.Encyclopedia.First(x =>
                                                    x.DigimonEvolutionId == digimonEvolutionInfo?.Id)));
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    _logger.Information($"Error: {e.Message}");
                                    _logger.Information($"Error: {e.StackTrace}");
                                }
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Information($"Error total: {e.Message}");
                    _logger.Information($"Error total: {e.StackTrace}");
                }
            });
            _logger.Debug(
                $"Added new information to all characters, Digimon count: {digimonCount}, Encyclopedia count: {encyclopediaCount}, Encyclopedia evolution count: {encyclopediaEvolutionCount}");
            return Task.CompletedTask;
        }
    }
}
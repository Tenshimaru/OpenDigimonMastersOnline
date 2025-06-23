using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Chat;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ChatMessagePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ChatMessage;

        private readonly GameMasterCommandsProcessor _gmCommands;
        private readonly PlayerCommandsProcessor _playerCommands;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public ChatMessagePacketProcessor(GameMasterCommandsProcessor gmCommands, PlayerCommandsProcessor playerCommands,
            MapServer mapServer, DungeonsServer dungeonServer, EventServer eventServer, PvpServer pvpServer,
            ILogger logger, ISender sender)
        {
            _gmCommands = gmCommands;
            _playerCommands = playerCommands;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            string message = packet.ReadString();

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            switch (client.AccessLevel)
            {
                case AccountAccessLevelEnum.Default:
                    {
                        if (message.StartsWith("!"))
                        {
                            _logger.Debug($"Tamer trys to execute \"{message}\".");
                            await _playerCommands.ExecuteCommand(client, message.TrimStart('!'));
                        }
                        else
                        {
                            _logger.Debug($"Tamer says \"{message}\" to NormalChat.");

                            switch (mapConfig?.Type)
                            {
                                case MapTypeEnum.Dungeon:
                                    _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId, new ChatMessagePacket(message, ChatTypeEnum.Normal, client.Tamer.GeneralHandler).Serialize());
                                    break;

                                case MapTypeEnum.Event:
                                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId, new ChatMessagePacket(message, ChatTypeEnum.Normal, client.Tamer.GeneralHandler).Serialize());
                                    break;

                                case MapTypeEnum.Pvp:
                                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId, new ChatMessagePacket(message, ChatTypeEnum.Normal, client.Tamer.GeneralHandler).Serialize());
                                    break;

                                default:
                                    {
                                        await _mapServer.CallDiscord(message, client, "00ff05", "C");
                                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new ChatMessagePacket(message, ChatTypeEnum.Normal, client.Tamer.GeneralHandler).Serialize());
                                    }
                                    break;
                            }

                            await _sender.Send(new CreateChatMessageCommand(ChatMessageModel.Create(client.TamerId, message)));
                        }
                    }
                    break;

                case AccountAccessLevelEnum.Blocked:
                    break;

                case AccountAccessLevelEnum.Moderator:
                case AccountAccessLevelEnum.GameMasterOne:
                case AccountAccessLevelEnum.GameMasterTwo:
                case AccountAccessLevelEnum.GameMasterThree:
                case AccountAccessLevelEnum.Administrator:
                    {
                        if (message.StartsWith("!"))
                        {
                            _logger.Debug($"Tamer trys to execute \"{message}\".");
                            await _gmCommands.ExecuteCommand(client, message.TrimStart('!'));
                        }
                        else
                        {
                            _logger.Debug($"Tamer says \"{message}\" to NormalChat.");

                            switch (mapConfig?.Type)
                            {
                                case MapTypeEnum.Dungeon:
                                    {
                                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId, new ChatMessagePacket(message, ChatTypeEnum.Normal, client.Tamer.GeneralHandler).Serialize());
                                    }
                                    break;

                                case MapTypeEnum.Event:
                                    {
                                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId, new ChatMessagePacket(message, ChatTypeEnum.Normal, client.Tamer.GeneralHandler).Serialize());
                                    }
                                    break;

                                case MapTypeEnum.Pvp:
                                    {
                                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId, new ChatMessagePacket(message, ChatTypeEnum.Normal, client.Tamer.GeneralHandler).Serialize());
                                    }
                                    break;

                                default:
                                    {
                                        await _mapServer.CallDiscord(message, client, "6b00ff", "STAFF");
                                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new ChatMessagePacket(message, ChatTypeEnum.Normal, client.Tamer.GeneralHandler).Serialize());
                                    }
                                    break;
                            }

                            await _sender.Send(new CreateChatMessageCommand(ChatMessageModel.Create(client.TamerId, message)));
                        }
                    }
                    break;

                default:
                    _logger.Warning($"Invalid Access Level for account {client.AccountId}.");
                    break;
            }
        }
    }
}
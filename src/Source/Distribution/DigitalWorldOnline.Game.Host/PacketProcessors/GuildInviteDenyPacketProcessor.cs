using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class GuildInviteDenyPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.GuildInviteDeny;

        private readonly MapServer _mapServer;
        private readonly EventServer _eventServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public GuildInviteDenyPacketProcessor(
            MapServer mapServer,
            EventServer eventServer,
            DungeonsServer dungeonsServer,
            PvpServer pvpServer,
            ILogger logger,
            ISender sender)
        {
            _mapServer = mapServer;
            _eventServer = eventServer;
            _dungeonsServer = dungeonsServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var guildId = packet.ReadInt();
            var senderName = packet.ReadString();

            _logger.Debug($"Searching character by name {senderName}...");
            var targetCharacter = await _sender.Send(new CharacterByNameQuery(senderName));
            if (targetCharacter != null)
            {
                _logger.Verbose($"Character {targetCharacter.Id} denied guild invitation from {client.Tamer.Id}.");

                _logger.Debug($"Sending guild invite deny packet for character id {targetCharacter.Id}...");
                var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(targetCharacter.Location.MapId));
                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        _dungeonsServer.BroadcastForUniqueTamer(targetCharacter.Id,
                            new GuildInviteDenyPacket(client.Tamer.Name).Serialize());
                        break;

                    case MapTypeEnum.Event:
                        _eventServer.BroadcastForUniqueTamer(targetCharacter.Id,
                            new GuildInviteDenyPacket(client.Tamer.Name).Serialize());
                        break;

                    case MapTypeEnum.Pvp:
                        _pvpServer.BroadcastForUniqueTamer(targetCharacter.Id,
                            new GuildInviteDenyPacket(client.Tamer.Name).Serialize());
                        break;

                    default:
                        _mapServer.BroadcastForUniqueTamer(targetCharacter.Id,
                            new GuildInviteDenyPacket(client.Tamer.Name).Serialize());
                        break;
                }
            }
            else
                _logger.Warning($"Character not found with name {senderName}.");
        }
    }
}
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class JumpBoosterPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.JumpBooster;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly IConfiguration _configuration;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        private const string GameServerAddress = "GameServer:Address";
        private const string GamerServerPublic = "GameServer:PublicAddress";
        private const string GameServerPort = "GameServer:Port";

        public JumpBoosterPacketProcessor(MapServer mapServer, DungeonsServer dungeonsServer, EventServer eventServer, PvpServer pvpServer,
            IConfiguration configuration, ISender sender, ILogger logger)
        {
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _configuration = configuration;
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var vipEnabled = Convert.ToBoolean(packet.ReadByte());
            var slot = packet.ReadShort();
            var mapId = packet.ReadShort();

            if (mapId == 1)
            {
                mapId = 3;
            }

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(mapId));
            var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(mapId));

            if (mapConfig == null || waypoints == null || !waypoints.Regions.Any())
            {
                client.Send(new SystemMessagePacket($"Map information not found for map Id {mapId}."));
                _logger.Warning($"Map information not found for map Id {mapId} on character {client.TamerId} jump booster.");
                return;
            }

            if (!vipEnabled)
            {
                var bombItem = client.Tamer.Inventory.FindItemBySlot(slot);

                if (!client.Tamer.Inventory.RemoveOrReduceItem(bombItem, 1, slot))
                {
                    client.Send(new SystemMessagePacket($"Unable to jump to {mapId}."));
                    _logger.Warning($"Invalid bomb item at slot {slot} for character {client.TamerId} jump booster.");
                    return;
                }

                await _sender.Send(new UpdateItemCommand(bombItem));

                _logger.Verbose($"Character {client.TamerId} jumped to map {mapId} with bomb {bombItem.ItemId}.");
            }
            else
                _logger.Verbose($"Character {client.TamerId} jumped to map {mapId} with VIP");

            if (client.PvpMap)
            {
                _pvpServer.RemoveClient(client);
            }
            else if (client.EventMap)
            {
                _eventServer.RemoveClient(client);
            }
            else
            {
                _mapServer.RemoveClient(client);
            }

            var destination = waypoints.Regions.First();

            client.Tamer.NewLocation(mapId, destination.X, destination.Y);
            await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

            client.Tamer.Partner.NewLocation(mapId, destination.X, destination.Y);
            await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));

            client.Tamer.UpdateState(CharacterStateEnum.Loading);
            await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

            client.SetGameQuit(false);

            client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y).Serialize());
        }
    }
}
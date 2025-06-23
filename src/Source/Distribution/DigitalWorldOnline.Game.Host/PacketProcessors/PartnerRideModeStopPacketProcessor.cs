using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartnerRideModeStopPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartnerRideModeStop;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonsServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;

        public PartnerRideModeStopPacketProcessor(
            MapServer mapServer,
            DungeonsServer dungeonsServer, 
            EventServer eventServer, 
            PvpServer pvpServer, 
            ILogger logger
            )
        {
            _mapServer = mapServer;
            _dungeonsServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
        }

        public Task Process(GameClient client, byte[] packetData)
        {
            client.Tamer.StopRideMode();

            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                new UpdateMovementSpeedPacket(client.Tamer).Serialize());

            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                new RideModeStopPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler).Serialize());

            _dungeonsServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                new UpdateMovementSpeedPacket(client.Tamer).Serialize());

            _dungeonsServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                new RideModeStopPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler).Serialize());

            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                new UpdateMovementSpeedPacket(client.Tamer).Serialize());

            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                new RideModeStopPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler).Serialize());

            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                new UpdateMovementSpeedPacket(client.Tamer).Serialize());

            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                new RideModeStopPacket(client.Tamer.GeneralHandler, client.Partner.GeneralHandler).Serialize());

            _logger.Verbose($"Character {client.TamerId} ended riding mode with " +
                $"{client.Partner.Id} ({client.Partner.CurrentType}).");

            return Task.CompletedTask;
        }
    }
}
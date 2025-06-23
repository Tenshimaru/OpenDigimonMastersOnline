using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ConnectionPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.Connection;

        private readonly int _handshakeDegree = 32321;
        private readonly ILogger _logger;

        public ConnectionPacketProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            _logger.Debug("Getting packet parameters...");
            var kind = packet.ReadByte();

            _logger.Debug($"Kind: {kind}");

            var handshakeTimestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var handshake = (short)(client.Handshake ^ _handshakeDegree);

            _logger.Debug($"Sending answer {handshake}, {handshakeTimestamp}");
            client.Send(new ConnectionPacket(handshake, handshakeTimestamp));

            return Task.CompletedTask;
        }
    }
}

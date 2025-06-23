using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.MapServer;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class SwitchChannelPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ChannelSwitchConfirm;

        private readonly ILogger _logger;

        public SwitchChannelPacketProcessor(
            ILogger logger)
        {
            _logger = logger;
        }

        public Task Process(GameClient client, byte[] packetData)
        {
            _logger.Debug($"Sending channel switch confirm packet...");
            client.Send(new ChannelSwitchConfirmPacket());

            return Task.CompletedTask;
        }
    }
}

using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class GuildHistoricLoadPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.GuildHistoric;

        private readonly ILogger _logger;

        public GuildHistoricLoadPacketProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public Task Process(GameClient client, byte[] packetData)
        {
            if (client.Tamer.Guild != null)
            {
                _logger.Debug($"Sending guild historic packet for character {client.TamerId}...");
                client.Send(new GuildHistoricPacket(client.Tamer.Guild.Historic));
            }

            return Task.CompletedTask;
        }
    }
}
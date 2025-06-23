using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Writers;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ArchiveAcademyPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ArchiveAcademyList;

        private readonly ILogger _logger;

        public ArchiveAcademyPacketProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packets = new GamePacketReader(packetData);

            _logger.Debug($"Reading Packet 3226 -> ArchiveAcademyList");

            /*var slot = 0;
            var itemId = 0;
            var remainTime = 0;

            var packet = new PacketWriter();

            packet.Type(3226);

            for (int i = 0; i < 3; i++)
            {
                packet.WriteInt(i);
                packet.WriteInt(10032);
                packet.WriteInt(5000);
            }

            client.Send(packet.Serialize());*/

            client.Send(new ArchiveAcademyIniciarPacket());
        }
    }
}


using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Writers;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class JoinEventQueuePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.JoinEventQueue;

        private readonly ILogger _logger;

        public JoinEventQueuePacketProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var writer = new PacketWriter();
            writer.Type(3124);
            writer.WriteString(DateTime.Now.AddSeconds(60).ToShortDateString());
            writer.WriteInt(0); //reset time

            writer.WriteInt(0); //Membros no time 1
            writer.WriteInt(0); //Membros no time 2

            writer.WriteInt(52); //minha pontuacao
            writer.WriteShort(62); //meu ranking
            writer.WriteByte(1); //meu time

            for (int i = 0; i < 20; i++) //para cada membro
            {
                writer.WriteByte(0); //rank do membro
            }

            client.Send(writer);
        }
    }
}
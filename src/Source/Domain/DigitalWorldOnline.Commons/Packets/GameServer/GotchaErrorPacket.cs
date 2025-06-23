using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GotchaErrorPacket : PacketWriter
    {
        private const int PacketNumber = 3959;

        /// <summary>
        /// Gotcha Error Packet
        /// </summary>
        public GotchaErrorPacket()
        {
            Type(PacketNumber);
            WriteUInt(1);
        }
    }
}
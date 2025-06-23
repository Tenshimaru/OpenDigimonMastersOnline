using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeFinalConfirmationPacket : PacketWriter
    {
        private const int PacketNumber = 1504;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="targetHandler">Target tamer handle from trade</param>
        public TradeFinalConfirmationPacket(int targetHandler)
        {
            Type(PacketNumber);
            WriteInt((int)targetHandler);
        }
    }
}
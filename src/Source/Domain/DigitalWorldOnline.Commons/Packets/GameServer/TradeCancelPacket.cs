using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeCancelPacket : PacketWriter
    {
        private const int PacketNumber = 1506;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="targetHandler">Target tamer handle from trade</param>
        public TradeCancelPacket(int targetHandler)
        {
            Type(PacketNumber);
            WriteInt((int)targetHandler);
        }
    }
}
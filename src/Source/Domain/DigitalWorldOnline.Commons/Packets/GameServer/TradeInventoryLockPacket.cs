using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeInventorylockPacket : PacketWriter
    {
        private const int PacketNumber = 1532;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="targetHandler">Target tamer handle from trade</param>
        public TradeInventorylockPacket(int targetHandler)
        {
            Type(PacketNumber);
            WriteInt((int)targetHandler);
            WriteByte(1);
        }
    }
}
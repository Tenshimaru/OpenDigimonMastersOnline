using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeInventoryUnlockPacket : PacketWriter
    {
        private const int PacketNumber = 1505;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="targetHandler">Target tamer handle from trade</param>
        public TradeInventoryUnlockPacket(int targetHandler)
        {
            Type(PacketNumber);
            WriteInt((int)targetHandler);
            WriteByte(0);
        }
    }
}
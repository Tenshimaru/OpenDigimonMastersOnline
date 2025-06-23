using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeConfirmationPacket : PacketWriter
    {
        private const int PacketNumber = 1503;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="targetHandler">Target tamer handle from trade</param>
        public TradeConfirmationPacket(int targetHandler)
        {
            Type(PacketNumber);
            WriteInt((int)targetHandler);
        }
    }
}
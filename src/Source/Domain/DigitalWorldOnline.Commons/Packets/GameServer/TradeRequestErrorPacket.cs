using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeRequestErrorPacket : PacketWriter
    {
        private const int PacketNumber = 1507;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="Result">Result type from trade request</param>
        public TradeRequestErrorPacket(TradeRequestErrorEnum Result )
        {
            Type(PacketNumber);
            WriteInt((int)Result);
        }
    }
}
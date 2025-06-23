using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeAcceptPacket : PacketWriter
    {
        private const int PacketNumber = 1502;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="TargetHandle">Target tamer handle accept</param>
        public TradeAcceptPacket(int TargetHandle)
        {
            Type(PacketNumber);
            WriteInt((int)TargetHandle);
        }
    }
}
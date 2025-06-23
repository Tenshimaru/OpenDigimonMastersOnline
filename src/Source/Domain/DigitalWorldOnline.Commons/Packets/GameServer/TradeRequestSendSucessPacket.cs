using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeRequestSucessPacket : PacketWriter
    {
        private const int PacketNumber = 1501;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="TargetHandle">Target tamer handle request</param>
        public TradeRequestSucessPacket(int TargetHandle)
        {
            Type(PacketNumber);
            WriteInt((int)TargetHandle);
        }
    }
}
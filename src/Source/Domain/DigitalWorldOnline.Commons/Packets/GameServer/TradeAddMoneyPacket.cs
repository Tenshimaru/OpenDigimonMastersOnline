using DigitalWorldOnline.Commons.Writers;
using System.Net.Sockets;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeAddMoneyPacket : PacketWriter
    {
        private const int PacketNumber = 1509;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="TargetHandle">Target tamer handle accept</param>
        public TradeAddMoneyPacket(int targetHandle, int TargetMoney)
        {
            Type(PacketNumber);
            WriteInt(targetHandle);
            WriteInt(TargetMoney);
        }
    }
}
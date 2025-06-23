using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TimeRechargeResultPacket : PacketWriter
    {
        private const int PacketNumber = 16042;

        /// <summary>
        /// Sends  Time Recharge Result.
        /// </summary>
        public TimeRechargeResultPacket()
        {
            Type(PacketNumber);
            WriteInt(0); // Result
            WriteShort((short)0); // Size
            WriteShort((short)0); // Count In Recharge

            //for (int i = 0; i < Tamer.RechargeItems.Count; i++)
            //{
            //    writter.WriteShort((short)Tamer.RechargeItems[i].slot);
            //    writter.WriteUInt(Tamer.RechargeItems[i].time_t);
            //    writter.WriteBytes(Tamer.RechargeItems[i].ToArray());
            //}
           
        }
    }
}
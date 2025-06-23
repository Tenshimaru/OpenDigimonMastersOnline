using DigitalWorldOnline.Commons.Models.Events;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TamerAttendancePacket : PacketWriter
    {
        private const int PacketNumber = 3133;

        /// <summary>
        /// Tamer's attendance event.
        /// </summary>
        /// <param name="tamerAttendance">Event info.</param>
        public TamerAttendancePacket(AttendanceRewardModel tamerAttendance)
        {
            Type(PacketNumber);
            WriteByte(0);
            WriteByte(tamerAttendance?.TotalDays ?? 0);
            WriteByte(0);
        }
        public TamerAttendancePacket(byte Days)
        {
            Type(PacketNumber);
            WriteByte(0);
            WriteByte(Days);
            WriteByte(0);
        }
    }
}

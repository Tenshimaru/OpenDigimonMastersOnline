using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class QuestDailyUpdatePacket : PacketWriter
    {
        private const int PacketNumber = 11006;

        /// <summary>
        /// Updates the daily quest flags.
        /// </summary>
        public QuestDailyUpdatePacket()
        {
            Type(PacketNumber);
        }
    }
}
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class QuestGoalUpdatePacket : PacketWriter
    {
        private const int PacketNumber = 11001;

        /// <summary>
        /// Updates the target quest current goal.
        /// </summary>
        /// <param name="questId">Quest unique identifier</param>
        /// <param name="goalIndex">Goal index</param>
        /// <param name="currentGoalValue">Goal current value</param>
        public QuestGoalUpdatePacket(short questId, byte goalIndex, short currentGoalValue)
        {
            Type(PacketNumber);
            WriteShort(questId);
            WriteByte(goalIndex);
            WriteShort(currentGoalValue);
        }
    }
}
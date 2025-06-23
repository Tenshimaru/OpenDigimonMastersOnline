using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class CastSkillPacket : PacketWriter
    {
        private const int PacketNumber = 1015;

        /// <summary>
        /// Uses the skill into the target.
        /// </summary>
        /// <param name="skillSlot">The skill source slot</param>
        /// <param name="attackerHandler">The source of the skill</param>
        /// <param name="targetHandler">The target of the skill</param>
        public CastSkillPacket(byte skillSlot, int attackerHandler, int targetHandler)
        {
            Type(PacketNumber);
            WriteByte(skillSlot);
            WriteShort((short)attackerHandler);
            WriteShort((short)targetHandler);
        }
    }
}
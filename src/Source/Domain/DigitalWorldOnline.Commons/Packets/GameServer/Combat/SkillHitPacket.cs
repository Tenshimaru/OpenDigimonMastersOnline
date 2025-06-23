using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class SkillHitPacket : PacketWriter
    {
        private const int PacketNumber = 1102;

        /// <summary>
        /// Uses the skill into the target.
        /// </summary>
        /// <param name="attackerHandler">The source of the skill</param>
        /// <param name="targetHandler">The target of the skill</param>
        /// <param name="skillSlot">The skill source slot</param>
        /// <param name="finalDamage">The final damage of the skill</param>
        /// <param name="targetCurrentHpRate">The final hp rate of the target</param>
        public SkillHitPacket(int attackerHandler, int targetHandler, byte skillSlot, int finalDamage, byte targetCurrentHpRate)
        {
            Type(PacketNumber);
            WriteByte(0);
            WriteInt(attackerHandler);
            WriteInt(targetHandler);
            WriteInt(skillSlot);
            WriteByte(255); //Max HP Rate?
            WriteByte(targetCurrentHpRate);
            WriteByte(18); //MaxExtStat
            WriteByte(4); //HP TODO: afetar outros status
            WriteInt(finalDamage * -1);
            WriteByte(255); //Max HP Rate?
            WriteByte(0);
        }
    }
}
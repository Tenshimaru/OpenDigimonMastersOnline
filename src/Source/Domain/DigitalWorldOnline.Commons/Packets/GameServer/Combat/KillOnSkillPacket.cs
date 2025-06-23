using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class KillOnSkillPacket : PacketWriter
    {
        private const int PacketNumber = 1021;

        /// <summary>
        /// Uses the skill into the target.
        /// </summary>
        /// <param name="attackerHandler">The source of the skill</param>
        /// <param name="targetHandler">The target of the skill</param>
        /// <param name="skillSlot">The skill source slot</param>
        /// <param name="finalDamage">The final damage of the skill</param>
        public KillOnSkillPacket(int attackerHandler, int targetHandler, byte skillSlot, int finalDamage)
        {
            Type(PacketNumber);
            WriteInt(attackerHandler);
            WriteInt(targetHandler);
            WriteInt(skillSlot);
            WriteInt(finalDamage * -1);
        }
    }
}
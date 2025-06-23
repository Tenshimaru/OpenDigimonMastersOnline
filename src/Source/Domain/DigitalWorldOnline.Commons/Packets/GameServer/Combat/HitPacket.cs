using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class HitPacket : PacketWriter
    {
        private const int PacketNumber = 1013;

        /// <summary>
        /// Attack the target with default hit.
        /// </summary>
        /// <param name="attackerHandler">The source of the hit</param>
        /// <param name="targetHandler">The target of the hit</param>
        /// <param name="finalDamage">The final damage of the hit</param>
        /// <param name="hpBeforeHit">The target hp before the hit</param>
        /// <param name="hpAfterHit">The target hp after the hit</param>
        /// <param name="hitType">Type of the hit (2-Block 1-Crit 0-Normal)</param>
        public HitPacket(int attackerHandler, int targetHandler, int finalDamage, int hpBeforeHit, int hpAfterHit, int hitType = 0)
        {
            Type(PacketNumber);
            WriteInt(attackerHandler);
            WriteInt(targetHandler);
            WriteInt(finalDamage * -1);
            WriteInt(hitType);
            WriteInt(hpAfterHit);
            WriteInt(hpBeforeHit);
            WriteShort(0);
        }
    }
}
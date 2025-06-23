using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class BlockHitPacket : PacketWriter
    {
        private const int PacketNumber = 1013;

        /// <summary>
        /// Blocks the default attack hit.
        /// </summary>
        /// <param name="attackerHandler">The source of the hit</param>
        /// <param name="targetHandler">The target of the hit</param>
        /// <param name="finalDamage">The final damage of the hit</param>
        /// <param name="hpBeforeHit">The target hp before the hit</param>
        /// <param name="hpAfterHit">The target hp after the hit</param>
        public BlockHitPacket(int attackerHandler, int targetHandler, int finalDamage, int hpBeforeHit, int hpAfterHit)
        {
            Type(PacketNumber);
            WriteInt(attackerHandler);
            WriteInt(targetHandler);
            WriteInt(finalDamage * -1);
            WriteInt(2);
            WriteInt(hpAfterHit);
            WriteInt(hpBeforeHit);
            WriteShort(0);
        }
    }
}
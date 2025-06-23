using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class KillOnHitPacket : PacketWriter
    {
        private const int PacketNumber = 1020;

        /// <summary>
        /// Attack the target with default hit.
        /// </summary>
        /// <param name="attackerHandler">The source of the hit</param>
        /// <param name="targetHandler">The target of the hit</param>
        /// <param name="damage">The damage of the hit</param>
        /// <param name="hitType">Type of the hit (2-Block 1-Crit 0-Normal)</param>
        public KillOnHitPacket(int attackerHandler, int targetHandler, int damage, int hitType = 0)
        {
            Type(PacketNumber);
            WriteInt(attackerHandler);
            WriteInt(targetHandler);
            WriteInt(damage * -1);
            WriteInt(hitType);
            WriteShort(0);
        }
    }
}
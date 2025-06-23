using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class MissHitPacket : PacketWriter
    {
        private const int PacketNumber = 1014;

        /// <summary>
        /// Miss the default hit.
        /// </summary>
        /// <param name="attackerHandler">The source of the hit</param>
        /// <param name="targetHandler">The target of the hit</param>
        public MissHitPacket(int attackerHandler, int targetHandler)
        {
            Type(PacketNumber);
            WriteInt(attackerHandler);
            WriteInt(targetHandler);
            WriteShort(0);
        }
    }
}
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    public class DestroyMobsPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Destroys the target mob.
        /// </summary>
        /// <param name="mob">The mob to destroy.</param>
        public DestroyMobsPacket(MobConfigModel mob)
        {
            Type(PacketNumber);
            WriteByte(2);
            WriteShort(1);
            WriteInt(mob.GeneralHandler);
        }
        public DestroyMobsPacket(SummonMobModel mob)
        {
            Type(PacketNumber);
            WriteByte(2);
            WriteShort(1);
            WriteInt(mob.GeneralHandler);
        }
    }
}
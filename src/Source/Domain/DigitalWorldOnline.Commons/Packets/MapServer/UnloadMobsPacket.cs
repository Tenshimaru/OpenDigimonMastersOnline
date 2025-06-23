using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    public class UnloadMobsPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Despawns the target mob.
        /// </summary>
        /// <param name="mob">The mob to despawn.</param>
        public UnloadMobsPacket(MobConfigModel mob)
        {
            Type(PacketNumber);
            WriteByte(4);
            WriteShort(1);
            WriteInt(mob.GeneralHandler);
            WriteInt(mob.CurrentLocation.X);
            WriteInt(mob.CurrentLocation.Y);
            WriteInt(0);
        }
        public UnloadMobsPacket(SummonMobModel mob)
        {
            Type(PacketNumber);
            WriteByte(4);
            WriteShort(1);
            WriteInt(mob.GeneralHandler);
            WriteInt(mob.CurrentLocation.X);
            WriteInt(mob.CurrentLocation.Y);
            WriteInt(0);
        }
    }
}
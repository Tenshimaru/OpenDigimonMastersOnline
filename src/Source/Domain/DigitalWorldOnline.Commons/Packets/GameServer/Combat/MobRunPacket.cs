using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Config.Events;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class MobRunPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Default mob movimentation packet.
        /// </summary>
        /// <param name="mob">The mob that is moving</param>
        public MobRunPacket(MobConfigModel mob)
        {
            Type(PacketNumber);
            WriteByte(6);
            WriteShort(1);
            WriteUInt((uint)mob.GeneralHandler);
            WriteInt(mob.CurrentLocation.X);
            WriteInt(mob.CurrentLocation.Y);
            WriteInt(0);
        }
        public MobRunPacket(SummonMobModel mob)
        {
            Type(PacketNumber);
            WriteByte(6);
            WriteShort(1);
            WriteUInt((uint)mob.GeneralHandler);
            WriteInt(mob.CurrentLocation.X);
            WriteInt(mob.CurrentLocation.Y);
            WriteInt(0);
        }
        public MobRunPacket(EventMobConfigModel mob)
        {
            Type(PacketNumber);
            WriteByte(6);
            WriteShort(1);
            WriteUInt((uint)mob.GeneralHandler);
            WriteInt(mob.CurrentLocation.X);
            WriteInt(mob.CurrentLocation.Y);
            WriteInt(0);
        }
        public MobRunPacket(int X,int Y, uint Handler)
        {
            Type(PacketNumber);
            WriteByte(6);
            WriteShort(1);
            WriteUInt((uint)Handler);
            WriteInt(X);
            WriteInt(Y);
            WriteInt(0);
        }
    }
}
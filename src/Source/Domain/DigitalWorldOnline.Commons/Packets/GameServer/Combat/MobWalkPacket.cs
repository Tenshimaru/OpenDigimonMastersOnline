using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Config.Events;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class MobWalkPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Default mob movimentation packet.
        /// </summary>
        /// <param name="mob">The mob that is moving</param>
        public MobWalkPacket(MobConfigModel mob)
        {
            Type(PacketNumber);
            WriteByte(5);
            WriteShort(1);
            WriteUInt((uint)mob.GeneralHandler);
            WriteInt(mob.CurrentLocation.X);
            WriteInt(mob.CurrentLocation.Y);
            WriteInt(0);
        }
        public MobWalkPacket(SummonMobModel mob)
        {
            Type(PacketNumber);
            WriteByte(5);
            WriteShort(1);
            WriteUInt((uint)mob.GeneralHandler);
            WriteInt(mob.CurrentLocation.X);
            WriteInt(mob.CurrentLocation.Y);
            WriteInt(0);
        }
        public MobWalkPacket(EventMobConfigModel mob)
        {
            Type(PacketNumber);
            WriteByte(5);
            WriteShort(1);
            WriteUInt((uint)mob.GeneralHandler);
            WriteInt(mob.CurrentLocation.X);
            WriteInt(mob.CurrentLocation.Y);
            WriteInt(0);
        }
        public MobWalkPacket(int X,int Y,uint Handler)
        {
            Type(PacketNumber);
            WriteByte(5);
            WriteShort(1);
            WriteUInt(Handler);
            WriteInt(X);
            WriteInt(Y);
            WriteInt(0);
        }
    }
}
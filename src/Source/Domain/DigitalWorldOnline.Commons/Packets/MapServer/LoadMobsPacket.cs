using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    //TODO: abstrair
    public enum SyncConditionEnum
    {
        NormalState = 0,
        CastingSkill = 6,
        PersonalShopPrepare = 8
    }

    public class LoadMobsPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Spawns the target mob.
        /// </summary>
        /// <param name="mob">The mob to spawn.</param>
        public LoadMobsPacket(IList<MobConfigModel> mobs)
        {
            Type(PacketNumber);
            WriteByte(3);
            WriteShort((short)mobs.Count);

            foreach (var mob in mobs)
            {
                WriteInt(mob.PreviousLocation.X);
                WriteInt(mob.PreviousLocation.Y);
                WriteInt(mob.GeneralHandler);
                WriteInt(mob.Type);
                WriteInt(mob.CurrentLocation.X);
                WriteInt(mob.CurrentLocation.Y);
                WriteByte(mob.CurrentHpRate);
                WriteShort(mob.Level);

                //TODO: Growth factor dinamico
                WriteShort(2); //Skill Idx
                WriteInt(mob.GrowStack);
                WriteInt(SyncConditionEnum.NormalState.GetHashCode());
                WriteInt(mob.DisposedObjects);
                WriteByte(0);
            }

            WriteInt(0);
        }
        public LoadMobsPacket(IList<SummonMobModel> mobs)
        {
            Type(PacketNumber);
            WriteByte(3);
            WriteShort((short)mobs.Count);

            foreach (var mob in mobs)
            {
                WriteInt(mob.PreviousLocation.X);
                WriteInt(mob.PreviousLocation.Y);
                WriteInt(mob.GeneralHandler);
                WriteInt(mob.Type);
                WriteInt(mob.CurrentLocation.X);
                WriteInt(mob.CurrentLocation.Y);
                WriteByte(mob.CurrentHpRate);
                WriteShort(mob.Level);

                //TODO: Growth factor dinamico
                WriteShort(2); //Skill Idx
                WriteInt(mob.GrowStack);
                WriteInt(SyncConditionEnum.NormalState.GetHashCode());
                WriteInt(mob.DisposedObjects);
                WriteByte(0);
            }

            WriteInt(0);
        }
        /// <summary>
        /// Spawns the target mob.
        /// </summary>
        /// <param name="mob">The mob to spawn.</param>
        public LoadMobsPacket(MobConfigModel mob)
        {
            Type(PacketNumber);
            WriteByte(mob.Respawn ? (byte)1 : (byte)3);
            WriteShort(1);

            if (mob.Respawn)
            {
                WriteInt(mob.Location.X);
                WriteInt(mob.Location.Y);
            }
            else
            {
                WriteInt(mob.PreviousLocation.X);
                WriteInt(mob.PreviousLocation.Y);
            }

            WriteInt(mob.GeneralHandler);
            WriteInt(mob.Type);

            if (mob.Respawn)
            {
                WriteInt(mob.Location.X);
                WriteInt(mob.Location.Y);
            }
            else
            {
                WriteInt(mob.CurrentLocation.X);
                WriteInt(mob.CurrentLocation.Y);
            }

            WriteByte(mob.CurrentHpRate);
            WriteShort(mob.Level);

            //TODO: Growth factor dinamico
            WriteShort(2); //Skill Idx
            WriteInt(mob.GrowStack);
            WriteInt(SyncConditionEnum.NormalState.GetHashCode());
            WriteInt(mob.DisposedObjects);
            WriteByte(0);

            WriteInt(0);
        }
        public LoadMobsPacket(SummonMobModel mob,bool spawn = false)
        {
            Type(PacketNumber);
            if (spawn)
            {
                WriteByte(1);
            }
            else
            {
                WriteByte(3);
            }

            WriteShort(1);


            WriteInt(mob.Location.X);
            WriteInt(mob.Location.Y);

            WriteInt(mob.GeneralHandler);
            WriteInt(mob.Type);


            WriteInt(mob.Location.X);
            WriteInt(mob.Location.Y);


            WriteByte(mob.CurrentHpRate);
            WriteShort(mob.Level);

            //TODO: Growth factor dinamico
            WriteShort(2); //Skill Idx
            WriteInt(mob.GrowStack);
            WriteInt(SyncConditionEnum.NormalState.GetHashCode());
            WriteInt(mob.DisposedObjects);
            WriteByte(0);

            WriteInt(0);
        }
    }
}
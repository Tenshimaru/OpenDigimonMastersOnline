using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Summon;
using System;

namespace DigitalWorldOnline.Commons.Models.Map.Dungeons
{
    public sealed partial class RoyalBaseMap : MapConfigModel
    {
        public bool IsSleipmonDead { get; private set; }
        public bool IsDynasmonDead { get; private set; }
        public bool IsCraniamonDead { get; private set; }
        public bool IsExamonDead { get; private set; }
        public bool IsUlForceDead { get; private set; }
        public bool IsLordKnightDead { get; private set; }
        public bool IsAllRaidsInFloorOneDied { get; private set; }
        public bool AllowUsingPortalFromFloorOneToFloorTwo { get; private set; }
        public int DeadRaidCount { get; private set; }
        public bool IsDexDorugoramon { get; private set; }
        public bool AllowUsingPortalFromFloorTwoToFloorThree { get; private set; }

        public RoyalBaseMap(short mapId, List<MobConfigModel> mobs) : base(mapId, mobs)
        {
            IsSleipmonDead = false;
            IsDynasmonDead = false;
            IsCraniamonDead = false;
            IsExamonDead = false;
            IsUlForceDead = false;
            IsLordKnightDead = false;
            IsAllRaidsInFloorOneDied = false;
            AllowUsingPortalFromFloorOneToFloorTwo = false;
            DeadRaidCount = 0;
            IsDexDorugoramon = false;
            AllowUsingPortalFromFloorTwoToFloorThree = false;
        }

        public int GetCurrentMobFloor(MobConfigModel mob)
        {
            if (Enum.IsDefined(typeof(RoyalBaseMonstersFloorOneEnum), mob.Type))
            {
                return 1;
            }
            else if (Enum.IsDefined(typeof(RoyalBaseMonstersFloorTwoEnum), mob.Type))
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public int GetCurrentMobFloor(SummonMobModel mob)
        {
            if (Enum.IsDefined(typeof(RoyalBaseMonstersFloorOneEnum), mob.Type))
            {
                return 1;
            }
            else if (Enum.IsDefined(typeof(RoyalBaseMonstersFloorTwoEnum), mob.Type))
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
    }
}
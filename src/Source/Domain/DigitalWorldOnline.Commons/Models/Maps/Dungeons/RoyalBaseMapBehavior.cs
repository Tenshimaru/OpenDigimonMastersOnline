using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Summon;

namespace DigitalWorldOnline.Commons.Models.Map.Dungeons
{
    public enum RoyalBaseMonstersFloorOneEnum
    {
        Sleipmon = 51126,
        Dynasmon = 51185,
        Craniamon = 51144,
        Examon = 51141,
        UlForce = 51132,
        LordKnight = 51120
    }
    
    public enum RoyalBaseMonstersFloorTwoEnum
    {
        DexDorugoramon = 51212
    }

    public partial class RoyalBaseMap
    {
        public void UpdateMonsterDead(MobConfigModel mob)
        {
            //Console.WriteLine($"Updating Mob as dead, MobType : {mob.Type}");

            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.Sleipmon)
            {
                SetSlepimonDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.Dynasmon)
            {
                SetDynasmonDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.Craniamon)
            {
                SetCraniamonDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.Examon)
            {
                SetExamonDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.UlForce)
            {
                SetUlForceDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.LordKnight)
            {
                SetLordKnightDead(true);
            }

            DeadRaidCount++;

            if (DeadRaidCount >= 3)
            {
                SetAllowUsingPortalFromFloorOneToFloorTwo(true);
            }
            else
            {
                SetAllowUsingPortalFromFloorOneToFloorTwo(false);
            }

            if (IsSleipmonDead && IsCraniamonDead && IsExamonDead && IsUlForceDead && IsLordKnightDead && !IsAllRaidsInFloorOneDied)
            {
                SetIsAllRaidsInFloorOneDied(true);
            }


            if (mob.Type == (int)RoyalBaseMonstersFloorTwoEnum.DexDorugoramon)
            {
                SetIsDexDorugoramonDead(true);
                SetAllowUsingPortalFromFloorTwoToFloorThree(true);
            }
        }

        public void UpdateMonsterDead(SummonMobModel mob)
        {
            //Console.WriteLine($"Updating Mob as dead, MobType : {mob.Type}");

            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.Sleipmon)
            {
                SetSlepimonDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.Dynasmon)
            {
                SetDynasmonDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.Craniamon)
            {
                SetCraniamonDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.Examon)
            {
                SetExamonDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.UlForce)
            {
                SetUlForceDead(true);
            }
            if (mob.Type == (int)RoyalBaseMonstersFloorOneEnum.LordKnight)
            {
                SetLordKnightDead(true);
            }

            DeadRaidCount++;

            if (DeadRaidCount >= 3)
            {
                SetAllowUsingPortalFromFloorOneToFloorTwo(true);
            }
            else
            {
                SetAllowUsingPortalFromFloorOneToFloorTwo(false);
            }
        }

        public void SetSlepimonDead(bool value)
        {
            IsSleipmonDead = value;
        }

        public void SetDynasmonDead(bool value)
        {
            IsDynasmonDead = value;
        }

        public void SetCraniamonDead(bool value)
        {
            IsCraniamonDead = value;
        }

        public void SetExamonDead(bool value)
        {
            IsExamonDead = value;
        }

        public void SetUlForceDead(bool value)
        {
            IsUlForceDead = value;
        }

        public void SetLordKnightDead(bool value)
        {
            IsLordKnightDead = value;
        }

        public void SetIsDexDorugoramonDead(bool value)
        {
            IsDexDorugoramon = value;
        }

        public void SetIsAllRaidsInFloorOneDied(bool value)
        {
            IsAllRaidsInFloorOneDied = value;
        }

        public void SetAllowUsingPortalFromFloorOneToFloorTwo(bool value)
        {
            AllowUsingPortalFromFloorOneToFloorTwo = value;
        }

        public void SetAllowUsingPortalFromFloorTwoToFloorThree(bool value)
        {
            AllowUsingPortalFromFloorTwoToFloorThree = value;
        }

    }
}

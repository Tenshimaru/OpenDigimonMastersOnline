using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.Map;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.ViewModel.Summons;

namespace DigitalWorldOnline.Commons.Models.Summon
{
    public sealed partial class SummonMobModel : StatusAssetModel, ICloneable
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference for digimon type.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Monster spawn duration.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Monster spawn amount.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Client reference for digimon model.
        /// </summary>
        public int Model { get; set; }

        /// <summary>
        /// Digimon name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Base digimon level.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// View range (from current position) for aggressive mobs.
        /// </summary>
        public int ViewRange { get; set; }

        /// <summary>
        /// Hunt range (from start position) for giveup on chasing targets.
        /// </summary>
        public int HuntRange { get; set; }


        /// <summary>
        /// Monster class type enumeration. 8 = Raid Boss
        /// </summary>
        public int Class { get; private set; }


        /// <summary>
        /// Mob reaction type.
        /// </summary>
        public DigimonReactionTypeEnum ReactionType { get; set; }

        /// <summary>
        /// Mob attribute.
        /// </summary>
        public DigimonAttributeEnum Attribute { get; set; }

        /// <summary>
        /// Mob element.
        /// </summary>
        public DigimonElementEnum Element { get; set; }

        /// <summary>
        /// Mob main family.
        /// </summary>
        public DigimonFamilyEnum Family1 { get; set; }

        /// <summary>
        /// Mob second family.
        /// </summary>
        public DigimonFamilyEnum Family2 { get; set; }

        /// <summary>
        /// Mob third family.
        /// </summary>
        public DigimonFamilyEnum Family3 { get; set; }

        /// <summary>
        /// Total Attack Speed value.
        /// </summary>
        public int ASValue { get; set; }

        /// <summary>
        /// Total Attack Range value.
        /// </summary>
        public int ARValue { get; set; }

        /// <summary>
        /// Total Attack value.
        /// </summary>
        public int ATValue { get; set; }

        /// <summary>
        /// Total Block value.
        /// </summary>
        public int BLValue { get; set; }

        /// <summary>
        /// Total Critical value.
        /// </summary>
        public int CTValue { get; set; } //TODO: separar CR e CD

        /// <summary>
        /// Total Defense value.
        /// </summary>
        public int DEValue { get; set; }

        /// <summary>
        /// Total DigiSoul value.
        /// </summary>
        public int DSValue { get; set; }

        /// <summary>
        /// Total Evasion value.
        /// </summary>
        public int EVValue { get; set; }

        /// <summary>
        /// Total Health value.
        /// </summary>
        public int HPValue { get; set; }

        /// <summary>
        /// Total Hit Rate value.
        /// </summary>
        public int HTValue { get; set; }

        /// <summary>
        /// Total Run Speed value.
        /// </summary>
        public int MSValue { get; set; }

        /// <summary>
        /// Total Walk Speed value.
        /// </summary>
        public int WSValue { get; set; }

        /// <summary>
        /// Drop config.
        /// </summary>
        public SummonMobDropRewardModel? DropReward { get; set; }

        /// <summary>
        /// Exp config.
        /// </summary>
        public SummonMobExpRewardModel? ExpReward { get; set; }

        public SummonMobLocationModel Location { get; set; }


        //Dynamic
        public MobActionEnum CurrentAction { get; private set; }
        public DateTime LastActionTime { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime LastSkillTryTime { get; private set; }
        public DateTime NextWalkTime { get; private set; }
        public DateTime AgressiveCheckTime { get; private set; }
        public DateTime ViewCheckTime { get; private set; }
        public DateTime LastHitTime { get; private set; }
        public DateTime LastSkillTime { get; private set; }
        public DateTime LastHealTime { get; private set; }
        public DateTime ChaseEndTime { get; private set; }
        public DateTime BattleStartTime { get; private set; } //TODO: utilizar no grow
        public DateTime LastHitTryTime { get; private set; }
        public DateTime LasSkillTryTime { get; private set; }
        public DateTime DieTime { get; private set; }
        public int GeneralHandler { get; private set; }
        public int TargetSummonHandler;
        public int CurrentHP { get; private set; }
        public int Cooldown { get; private set; }
        public Location CurrentLocation { get; private set; }
        public Location PreviousLocation { get; private set; }
        public Location InitialLocation { get; private set; }
        public byte MoveCount { get; private set; }
        public byte GrowStack { get; set; }
        public byte DisposedObjects { get; set; }
        public bool InBattle { get; private set; }
        public bool AwaitingKillSpawn { get; private set; }
        public bool Dead { get; private set; }
        public bool Respawn { get; private set; }
        public bool CheckSkill { get; private set; }
        public bool IsPossibleSkill => ((double)CurrentHP / HPValue) * 100 <= 90;
        public bool BossMonster { get; private set; } //TODO: ajustar valor conforme type
        public List<CharacterModel> TargetTamers { get; private set; }
        public Dictionary<long, int> RaidDamage { get; private set; }
        public List<long> TamersViewing { get; private set; }

        public void initialLocation()
        {
            CurrentLocation = Location;
            PreviousLocation = Location;
            InitialLocation = Location;
        }
        public SummonMobModel()
        {
            TamersViewing = new List<long>();
            RaidDamage = new Dictionary<long, int>();
            TargetTamers = new List<CharacterModel>();
            Location = new SummonMobLocationModel();

            Duration = 0;

            CurrentAction = MobActionEnum.Wait;
            LastActionTime = DateTime.Now;
            AgressiveCheckTime = DateTime.Now;
            ViewCheckTime = DateTime.Now;
            NextWalkTime = DateTime.Now;
            ChaseEndTime = DateTime.Now;
            LastHealTime = DateTime.Now;
            LastHitTime = DateTime.Now;
            LastHitTryTime = DateTime.Now;
            LastSkillTryTime = DateTime.Now;
            DieTime = DateTime.Now;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.Map;
using DigitalWorldOnline.Commons.Models.Character;

namespace DigitalWorldOnline.Commons.Models.Config.Events
{
    public sealed partial class EventMobConfigModel : StatusAssetModel, ICloneable
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long EventMapConfigId { get; set; }

        /// <summary>
        /// Client reference for digimon type.
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Client reference for digimon model.
        /// </summary>
        public int Model { get; private set; }

        /// <summary>
        /// Digimon name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Base digimon level.
        /// </summary>
        public byte Level { get; private set; }

        /// <summary>
        /// Digimon scaling type.
        /// </summary>
        public byte ScaleType { get; private set; }

        /// <summary>
        /// View range (from current position) for aggressive mobs.
        /// </summary>
        public int ViewRange { get; private set; }

        /// <summary>
        /// Hunt range (from start position) for giveup on chasing targets.
        /// </summary>
        public int HuntRange { get; private set; }

        /// <summary>
        /// Monster class type enumeration. 8 = Raid Boss
        /// </summary>
        public int Class { get; private set; }

        /// <summary>
        /// Monster coliseum Round
        /// </summary>
        public byte Round { get; set; }

        /// <summary>
        /// Digimon reaction type.
        /// </summary>
        public DigimonReactionTypeEnum ReactionType { get; private set; }

        /// <summary>
        /// Digimon attribute.
        /// </summary>
        public DigimonAttributeEnum Attribute { get; private set; }

        /// <summary>
        /// Digimon element.
        /// </summary>
        public DigimonElementEnum Element { get; private set; }

        /// <summary>
        /// Digimon main family.
        /// </summary>
        public DigimonFamilyEnum Family1 { get; private set; }

        /// <summary>
        /// Digimon second family.
        /// </summary>
        public DigimonFamilyEnum Family2 { get; private set; }

        /// <summary>
        /// Digimon third family.
        /// </summary>
        public DigimonFamilyEnum Family3 { get; private set; }

        /// <summary>
        /// Respawn interval in seconds.
        /// </summary>
        public int RespawnInterval { get; private set; }

        /// <summary>
        /// Monster spawn duration.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Initial location.
        /// </summary>
        public MobLocationConfigModel Location { get; private set; }

        /// <summary>
        /// Drop config.
        /// </summary>
        public MobDropRewardConfigModel? DropReward { get; private set; }

        /// <summary>
        /// Exp config.
        /// </summary>
        public MobExpRewardConfigModel? ExpReward { get; private set; }

        public MobDebuffListModel DebuffList { get; private set; }

        //Dynamic
        public MobActionEnum CurrentAction { get; private set; }
        public DateTime LastActionTime { get; private set; }
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
        public DateTime? DeathTime { get; set; }
        public DateTime? ResurrectionTime { get; set; }
        public int GeneralHandler { get; private set; }
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
        public bool Bless => Type == 45161 && Location.MapId == 1307;
        public bool BossMonster { get; private set; } //TODO: ajustar valor conforme type
        public List<CharacterModel> TargetTamers { get; private set; }
        public Dictionary<long, int> RaidDamage { get; private set; }
        public List<long> TamersViewing { get; private set; }
        
        public EventMapsConfigModel EventMapConfig { get; set; }

        public EventMobConfigModel()
        {
            TamersViewing = new List<long>();
            RaidDamage = new Dictionary<long, int>();
            TargetTamers = new List<CharacterModel>();
            Location = new MobLocationConfigModel();
            RespawnInterval = 8;
            DebuffList = new MobDebuffListModel();
            CurrentAction = MobActionEnum.Wait;
            Duration = 0;
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
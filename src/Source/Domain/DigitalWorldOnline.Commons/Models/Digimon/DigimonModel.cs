using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Character;
using System.Text.Json.Serialization;

namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonModel
    {
        //Temp
        public DateTime tempUpdating { get; set; } = DateTime.Now;
        public bool tempRecalculate { get; set; }
        public bool tempCalculating { get; set; }

        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Digimon base type.
        /// </summary>
        public int BaseType { get; private set; }

        /// <summary>
        /// Digimon current model.
        /// </summary>
        public int Model { get; private set; }

        /// <summary>
        /// Digimon current level.
        /// </summary>
        public byte Level { get; private set; }

        /// <summary>
        /// Digimon name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Digimon current size.
        /// </summary>
        public short Size { get; private set; }

        /// <summary>
        /// Digimon current experience.
        /// </summary>
        public long CurrentExperience { get; private set; }

        /// <summary>
        /// Digimon current skill experience.
        /// </summary>
        public long CurrentSkillExperience { get; private set; }

        /// <summary>
        /// Digimon current transcendence experience.
        /// </summary>
        public long TranscendenceExperience { get; private set; }

        /// <summary>
        /// Digimon creation date.
        /// </summary>
        public DateTime CreateDate { get; private set; }

        public DateTime LastSkillsCheck { get; private set; }

        /// <summary>
        /// Digimon hatch grade.
        /// </summary>
        public DigimonHatchGradeEnum HatchGrade { get; private set; }

        /// <summary>
        /// Digimon current type.
        /// </summary>
        public int CurrentType { get; private set; }

        /// <summary>
        /// Digimon friendship value.
        /// </summary>
        public byte Friendship { get; private set; }

        /// <summary>
        /// Digimon active buff list.
        /// </summary>
        public DigimonBuffListModel BuffList { get; private set; }
    
        public DigimonDebuffListModel DebuffList { get; private set; }
        /// <summary>
        /// Digimon digiclone details.
        /// </summary>
        public DigimonDigicloneModel Digiclone { get; private set; }

        /// <summary>
        /// Digimon available evolutions.
        /// </summary>
        public List<DigimonEvolutionModel> Evolutions { get; private set; }

        /// <summary>
        /// Digimon attribute experience.
        /// </summary>
        public DigimonAttributeExperienceModel AttributeExperience { get; private set; }

        /// <summary>
        /// Digimon current location.
        /// </summary>
        public DigimonLocationModel Location { get; set; }

        /// <summary>
        /// Digimon evolution status.
        /// </summary>
        public StatusAssetModel BaseStatus { get; private set; }
        
        /// <summary>
        /// Digimon base information.
        /// </summary>
        public DigimonBaseInfoAssetModel BaseInfo { get; private set; }

        /// <summary>
        /// Digimon seal status list.
        /// </summary>
        public List<SealDetailAssetModel> SealStatusList { get; private set; } = new();

        /// <summary>
        /// Digimon title status.
        /// </summary>
        public TitleStatusAssetModel? TitleStatus { get; private set; }

        public byte Slot { get; private set; }

        //Dynamic
        public bool AutoAttack { get; private set; }

        public bool HasActiveSkills()
        {
            return Evolutions.Any(x => x.Type > 0 && x.Skills.Any(s => s.Duration > 0));
        }

       

        public DateTime LastHitTime { get; private set; }
        public DateTime EndAttacking { get; private set; }
        public DateTime EndCasting { get; private set; }

        public Location ViewLocation { get; private set; }
        public Location BeforeEvent { get; private set; }
        public ConditionEnum CurrentCondition { get; private set; }
        public ConditionEnum PreviousCondition { get; private set; }

        [JsonIgnore]
        public CharacterModel? Character { get; private set; }
        public long? CharacterId { get; private set; }

        public DigimonModel()
        {
            BuffList = new DigimonBuffListModel();
            Digiclone = new DigimonDigicloneModel();
            Evolutions = new List<DigimonEvolutionModel>();
            AttributeExperience = new DigimonAttributeExperienceModel();
            ViewLocation = new Location();
            BeforeEvent = new Location();
            BaseStatus = new StatusAssetModel();
            DebuffList = new DigimonDebuffListModel();
            CurrentCondition = ConditionEnum.Default;
        }
    }
}
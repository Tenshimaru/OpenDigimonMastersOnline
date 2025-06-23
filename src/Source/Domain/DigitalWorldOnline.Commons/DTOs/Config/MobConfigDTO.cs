using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public sealed class MobConfigDTO : StatusDTO, ICloneable
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
        public int Class { get; set; }

        /// <summary>
        /// Monster coliseum Type
        /// </summary>
        public bool Coliseum { get; set; }

        /// <summary>
        /// Monster coliseum Round
        /// </summary>
        public byte Round { get; set; }

        public DungeonDayOfWeekEnum WeekDay { get; set; }

        /// <summary>
        /// Mob Coliseum  type.
        /// </summary>
        public ColiseumMobTypeEnum ColiseumMobType { get; set; }

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
        /// Respawn interval in seconds.
        /// </summary>
        public int RespawnInterval { get; set; }

        /// <summary>
        /// Initial location.
        /// </summary>
        public MobLocationConfigDTO Location { get; set; }

        /// <summary>
        /// Drop config.
        /// </summary>
        public MobDropRewardConfigDTO? DropReward { get; set; }

        /// <summary>
        /// Exp config.
        /// </summary>
        public MobExpRewardConfigDTO? ExpReward { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long GameMapConfigId { get; set; }
        public MapConfigDTO GameMapConfig { get; set; }

        public DateTime? DeathTime { get; set; }

        public DateTime? ResurrectionTime { get; set; }


        public object Clone()
        {
            var clonedObject = (MobConfigDTO)CloneMob();

            return clonedObject;
        }

        private object CloneMob()
        {
            return (MobConfigDTO)MemberwiseClone();
        }
    }
}

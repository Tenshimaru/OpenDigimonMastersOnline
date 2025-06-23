using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class SummonMobDTO : StatusDTO
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
        public long SummonDTOId { get; set; }

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
        public SummonMobDropRewardDTO DropReward { get; set; }

        /// <summary>
        /// Exp config.
        /// </summary>
        public SummonMobExpRewardDTO ExpReward { get; set; }

        public SummonMobLocationDTO Location { get; set; }
        public object Clone()
        {
            var clonedObject = (SummonMobDTO)CloneMob();

            return clonedObject;
        }

        private object CloneMob()
        {
            return (SummonMobDTO)MemberwiseClone();
        }

    }
}
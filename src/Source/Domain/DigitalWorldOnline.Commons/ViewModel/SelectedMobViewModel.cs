using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.ViewModel
{
    public class SelectedMobViewModel
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
        public int CTValue { get; set; }

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
        /// View range (from current position) for aggressive mobs.
        /// </summary>
        public int ViewRange { get; set; }

        /// <summary>
        /// Hunt range (from start position) for giveup on chasing targets.
        /// </summary>
        public int HuntRange { get; set; }

        /// <summary>
        /// Monster coliseum Type
        /// </summary>
        public bool Coliseum { get; set; }

        /// <summary>
        /// Monster coliseum Round
        /// </summary>
        public byte Round { get; set; }

        /// <summary>
        /// Mob Coliseum  type.
        /// </summary>
        public ColiseumMobTypeEnum ColiseumMobType { get; set; }

        public DungeonDayOfWeekEnum WeekDay { get; set; }

        /// <summary>
        /// Digimon reaction type.
        /// </summary>
        public DigimonReactionTypeEnum ReactionType { get; set; }

        /// <summary>
        /// Digimon attribute.
        /// </summary>
        public DigimonAttributeEnum Attribute { get; set; }

        /// <summary>
        /// Digimon element.
        /// </summary>
        public DigimonElementEnum Element { get; set; }

        /// <summary>
        /// Digimon main family.
        /// </summary>
        public DigimonFamilyEnum Family1 { get; set; }

        /// <summary>
        /// Digimon second family.
        /// </summary>
        public DigimonFamilyEnum Family2 { get; set; }

        /// <summary>
        /// Digimon third family.
        /// </summary>
        public DigimonFamilyEnum Family3 { get; set; }
    }
}

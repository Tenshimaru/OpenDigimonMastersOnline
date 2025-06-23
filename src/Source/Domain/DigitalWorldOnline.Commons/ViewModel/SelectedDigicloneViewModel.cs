using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.ViewModel
{
    public sealed class SelectedDigicloneViewModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the Digiclone item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Enchant level for this clone.
        /// </summary>
        public byte EnchantLevel { get; set; }

        /// <summary>
        /// Enchant status enumeration.
        /// </summary>
        public DigicloneTypeEnum DigicloneEnchantStatus { get; set; }

        /// <summary>
        /// Minimal enchant value when succeed.
        /// </summary>
        public short MinimalValue { get; set; }

        /// <summary>
        /// Maximum enchant value when succeed.
        /// </summary>
        public short MaximumValue { get; set; }

        /// <summary>
        /// Success chance upon cloning.
        /// </summary>
        public double UpgradeChance { get; set; }

        /// <summary>
        /// Break chance upon failing.
        /// </summary>
        public double BreakChance { get; set; }

        /// <summary>
        /// Price value in bits.
        /// </summary>
        public long Price { get; set; }

        /// <summary>
        /// Dynamic item name value.
        /// </summary>
        public string Name { get; set; }
    }
}
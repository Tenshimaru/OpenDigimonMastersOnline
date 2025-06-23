using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class CloneValueAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Clon type.
        /// </summary>
        public DigicloneTypeEnum Type { get; private set; }

        /// <summary>
        /// Min applyable clon level.
        /// </summary>
        public byte MinLevel { get; private set; }

        /// <summary>
        /// Max applyable clon level.
        /// </summary>
        public byte MaxLevel { get; private set; }

        /// <summary>
        /// Minimun value.
        /// </summary>
        public int MinValue { get; private set; }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public int MaxValue { get; private set; }
    }
}
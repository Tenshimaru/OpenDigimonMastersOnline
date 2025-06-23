using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class CloneValueAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Clon type.
        /// </summary>
        public DigicloneTypeEnum Type { get; set; }
        
        /// <summary>
        /// Min applyable clon level.
        /// </summary>
        public byte MinLevel { get; set; }

        /// <summary>
        /// Max applyable clon level.
        /// </summary>
        public byte MaxLevel { get; set; }

        /// <summary>
        /// Minimun value.
        /// </summary>
        public int MinValue { get; set; }
        
        /// <summary>
        /// Maximum value.
        /// </summary>
        public int MaxValue { get; set; }
    }
}
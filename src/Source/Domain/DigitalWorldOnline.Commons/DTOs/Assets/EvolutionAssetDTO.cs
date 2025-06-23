using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class EvolutionAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Base type for the evolution.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Evolution rank enumeration.
        /// </summary>
        public EvolutionRankEnum EvolutionRank { get; set; }

        /// <summary>
        /// Evolution lines.
        /// </summary>
        public List<EvolutionLineAssetDTO> Lines { get; set; }
    }
}
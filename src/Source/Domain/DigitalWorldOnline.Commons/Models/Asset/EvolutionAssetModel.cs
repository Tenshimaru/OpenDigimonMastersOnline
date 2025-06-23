using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class EvolutionAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Base type for the evolution.
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Evolution rank enumeration.
        /// </summary>
        public EvolutionRankEnum EvolutionRank { get; private set; }

        /// <summary>
        /// Evolution lines.
        /// </summary>
        public List<EvolutionLineAssetModel> Lines { get; private set; }
    }
}
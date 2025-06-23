namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class EvolutionStageAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Stage type.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Stage value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Refernece to the owner.
        /// </summary>
        public long EvolutionLineId { get; set; }
        public EvolutionLineAssetDTO EvolutionLine { get; set; }
    }
}
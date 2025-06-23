namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class CloneAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference to the target item section.
        /// </summary>
        public int ItemSection { get; set; }

        /// <summary>
        /// Min applyable clon level.
        /// </summary>
        public byte MinLevel { get; set; }

        /// <summary>
        /// Max applyable clon level.
        /// </summary>
        public byte MaxLevel { get; set; }

        /// <summary>
        /// Cost in bits.
        /// </summary>
        public long Bits { get; set; }
    }
}
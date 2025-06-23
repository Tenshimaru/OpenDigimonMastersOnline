namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed partial class CloneAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Client reference to the target item section.
        /// </summary>
        public int ItemSection { get; private set; }

        /// <summary>
        /// Min applyable clon level.
        /// </summary>
        public byte MinLevel { get; private set; }

        /// <summary>
        /// Max applyable clon level.
        /// </summary>
        public byte MaxLevel { get; private set; }

        /// <summary>
        /// Cost in bits.
        /// </summary>
        public long Bits { get; private set; }
    }
}
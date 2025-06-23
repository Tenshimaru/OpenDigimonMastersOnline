namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class NpcItemAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the target item.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long NpcAssetId { get; set; }
        public NpcAssetDTO NpcAsset { get; set; }
    }
}
namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class ItemCraftMaterialAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the material item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Required amount.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long ItemCraftId { get; set; }
        public ItemCraftAssetDTO ItemCraft { get; set; }
    }
}
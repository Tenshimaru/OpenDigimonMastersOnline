namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class ItemCraftMaterialAssetModel
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
    }
}
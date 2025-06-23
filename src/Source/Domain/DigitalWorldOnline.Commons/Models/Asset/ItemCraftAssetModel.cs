namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class ItemCraftAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference to the target NPC item.
        /// </summary>
        public int SequencialId { get; set; }

        /// <summary>
        /// Reference to the craftable item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Target NPC id.
        /// </summary>
        public int NpcId { get; set; }

        /// <summary>
        /// Craft chance for the target item.
        /// </summary>
        public byte SuccessRate { get; set; }

        /// <summary>
        /// Craft price in bits.
        /// </summary>
        public long Price { get; set; }

        /// <summary>
        /// Total given amount of the target item.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Required materials for the craft.
        /// </summary>
        public List<ItemCraftMaterialAssetModel> Materials { get; set; }
    }
}
namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class ScanDetailAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Min. amount of rewards.
        /// </summary>
        public byte MinAmount { get; set; }

        /// <summary>
        /// Max. amount of rewards.
        /// </summary>
        public byte MaxAmount { get; set; }

        /// <summary>
        /// Client reference to the item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Refered item name.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Available rewards.
        /// </summary>
        public List<ScanRewardDetailAssetModel> Rewards { get; set; }
    }
}
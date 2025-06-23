namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class ContainerAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Client item reference.
        /// </summary>
        public int ItemId { get; private set; }

        /// <summary>
        /// Refered item name.
        /// </summary>
        public string ItemName { get; private set; }

        /// <summary>
        /// Min. amount of rewards.
        /// </summary>
        public int RewardAmount { get; private set; }

        /// <summary>
        /// Available rewards
        /// </summary>
        public List<ContainerRewardAssetModel> Rewards { get; private set; }
    }
}
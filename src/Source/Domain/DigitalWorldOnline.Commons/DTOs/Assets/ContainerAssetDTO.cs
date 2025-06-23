namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class ContainerAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client item reference.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Refered item name.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Min. amount of rewards.
        /// </summary>
        public int RewardAmount { get; set; }

        /// <summary>
        /// Available rewards
        /// </summary>
        public List<ContainerRewardAssetDTO> Rewards { get; set; }
    }
}
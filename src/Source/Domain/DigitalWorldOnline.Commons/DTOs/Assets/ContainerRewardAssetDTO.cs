namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class ContainerRewardAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference to the item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Refered item name.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Minimal reward amount.
        /// </summary>
        public int MinAmount { get; set; }

        /// <summary>
        /// Maximum reward amount.
        /// </summary>
        public int MaxAmount { get; set; }

        /// <summary>
        /// Reward aquire chance.
        /// </summary>
        public double Chance { get; set; }

        /// <summary>
        /// Flag for global message display.
        /// </summary>
        public bool Rare { get; set; }

        /// <summary>
        /// Refernece to the owner.
        /// </summary>
        public long ContainerAssetId { get; set; }
        public ContainerAssetDTO ContainerAsset { get; set; }
    }
}
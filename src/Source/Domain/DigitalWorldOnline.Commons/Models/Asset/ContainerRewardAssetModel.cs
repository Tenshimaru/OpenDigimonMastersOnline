namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class ContainerRewardAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Client reference to the item id.
        /// </summary>
        public int ItemId { get; private set; }

        /// <summary>
        /// Refered item name.
        /// </summary>
        public string ItemName { get; private set; }

        /// <summary>
        /// Minimal reward amount.
        /// </summary>
        public int MinAmount { get; private set; }

        /// <summary>
        /// Maximum reward amount.
        /// </summary>
        public int MaxAmount { get; private set; }

        /// <summary>
        /// Reward aquire chance.
        /// </summary>
        public double Chance { get; private set; }

        /// <summary>
        /// Flag for global message display.
        /// </summary>
        public bool Rare { get; private set; }

        /// <summary>
        /// Refernece to the owner.
        /// </summary>
        public long ContainerAssetId { get; private set; }
        public ContainerAssetModel ContainerAsset { get; private set; }
    }
}
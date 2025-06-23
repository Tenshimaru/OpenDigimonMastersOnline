namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class AccessoryRollStatusAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Status type.
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Min status value.
        /// </summary>
        public int MinValue { get; private set; }

        /// <summary>
        /// Max status value.
        /// </summary>
        public int MaxValue { get; private set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long AccessoryRollAssetId { get; private set; }
        public AccessoryRollAssetModel AccessoryRollAsset { get; private set; }
    }
}
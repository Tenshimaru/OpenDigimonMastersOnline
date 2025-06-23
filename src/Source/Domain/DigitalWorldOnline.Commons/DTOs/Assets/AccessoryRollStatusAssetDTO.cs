namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class AccessoryRollStatusAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Status type.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Min status value.
        /// </summary>
        public int MinValue { get; set; }
        
        /// <summary>
        /// Max status value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long AccessoryRollAssetId { get; set; }
        public AccessoryRollAssetDTO AccessoryRollAsset { get; set; }
    }
}
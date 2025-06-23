namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class NpcPortalAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the portal type.
        /// </summary>
        public int PortalType { get; set; }

        /// <summary>
        /// Reference to the portal count.
        /// </summary>
        public int PortalCount { get; set; }

        public List<NpcPortalsAmountAssetModel> PortalsAsset { get; set; }
      
    }
}
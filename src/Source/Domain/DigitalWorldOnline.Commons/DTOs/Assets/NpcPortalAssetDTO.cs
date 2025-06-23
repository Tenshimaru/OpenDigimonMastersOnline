namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class NpcPortalAssetDTO
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

        public List<NpcPortalsAmountAssetDTO> PortalsAsset { get; set; }
        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long NpcAssetId { get; set; }
        public NpcAssetDTO NpcAsset { get; set; }
    }
}
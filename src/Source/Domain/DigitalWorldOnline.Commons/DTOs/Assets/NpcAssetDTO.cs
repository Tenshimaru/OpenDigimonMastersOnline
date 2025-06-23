namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class NpcAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the target NPC.
        /// </summary>
        public int NpcId { get; set; }
        
        /// <summary>
        /// Reference to the target map.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Available item list.
        /// </summary>
        public List<NpcItemAssetDTO> Items { get; set; }

        /// <summary>
        /// Available item list.
        /// </summary>
        public List<NpcPortalAssetDTO> Portals { get; set; }
    }
}
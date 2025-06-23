using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class NpcPortalsAmountAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }
     
        public List<NpcPortalsAssetDTO> npcPortalsAsset { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long NpcAssetId { get; set; }
        public NpcPortalAssetDTO NpcAsset { get; set; }
    }
}
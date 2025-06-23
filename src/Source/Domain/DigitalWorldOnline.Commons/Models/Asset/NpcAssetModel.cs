using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class NpcAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference to the target NPC.
        /// </summary>
        public int NpcId { get;  set; }
        
        /// <summary>
        /// Reference to the target map.
        /// </summary>
        public int MapId { get; private set; }

        /// <summary>
        /// Available item list.
        /// </summary>
        public List<NpcItemAssetModel> Items { get; set; }

        /// <summary>
        /// Available item list.
        /// </summary>
        public List<NpcPortalAssetModel> Portals { get; set; }
    }
}
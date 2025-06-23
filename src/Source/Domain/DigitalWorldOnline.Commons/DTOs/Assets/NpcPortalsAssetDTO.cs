using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class NpcPortalsAssetDTO
    {
        public long Id { get; set; }

        /// <summary>
        /// Reference to the portal type.
        /// </summary>
        public NpcResourceTypeEnum Type { get; set; }

        /// <summary>
        /// Reference to the portal count.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Reference to the portal count.
        /// </summary>
        public int ResourceAmount { get; set; }

        public long NpcAssetId { get; set; }

        public NpcPortalsAmountAssetDTO NpcAsset { get; set; }
    }
}

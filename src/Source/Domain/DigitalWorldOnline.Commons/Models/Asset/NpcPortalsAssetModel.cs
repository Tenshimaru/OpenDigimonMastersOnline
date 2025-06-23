using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
   public sealed partial  class NpcPortalsAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
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
    }
}

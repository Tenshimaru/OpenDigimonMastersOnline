

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed partial class XaiAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the client's item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Total XGauge.
        /// </summary>
        public int XGauge { get; set; }

        /// <summary>
        /// Total XCrystals.
        /// </summary>
        public short XCrystals { get; set; }
    }
}

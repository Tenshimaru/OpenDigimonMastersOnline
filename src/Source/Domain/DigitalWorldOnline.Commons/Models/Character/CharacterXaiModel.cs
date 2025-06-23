namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterXaiModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Client reference item id.
        /// </summary>
        public int ItemId { get; private set; }

        /// <summary>
        /// Xai max XGauge.
        /// </summary>
        public int XGauge { get; private set; }

        /// <summary>
        /// Xai max XCrystals.
        /// </summary>
        public short XCrystals { get; private set; }
    }
}
namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class HatchAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference to the target item.
        /// </summary>
        public int ItemId { get; private set; }

        /// <summary>
        /// Reference to the target digimon.
        /// </summary>
        public int HatchType { get; private set; }

        /// <summary>
        /// Section low class data type.
        /// </summary>
        public int LowClassDataSection { get; private set; }

        /// <summary>
        /// Section mid class data type.
        /// </summary>
        public int MidClassDataSection { get; private set; }

        /// <summary>
        /// Low class data insert amount.
        /// </summary>
        public int LowClassDataAmount { get; private set; }

        /// <summary>
        /// Mid class data insert amount.
        /// </summary>
        public int MidClassDataAmount { get; private set; }

        /// <summary>
        /// Low class data limit level.
        /// </summary>
        public int LowClassLimitLevel { get; private set; }

        /// <summary>
        /// Mid class data limit level.
        /// </summary>
        public int MidClassLimitLevel { get; private set; }

        /// <summary>
        /// Low class data insertion break point.
        /// </summary>
        public int LowClassBreakPoint { get; private set; }

        /// <summary>
        /// Low class data insertion break point.
        /// </summary>
        public int MidClassBreakPoint { get; private set; }

    }
}
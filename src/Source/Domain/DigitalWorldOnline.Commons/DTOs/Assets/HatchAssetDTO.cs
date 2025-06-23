namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class HatchAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the target item.
        /// </summary>
        public int ItemId { get; set; }
        
        /// <summary>
        /// Reference to the target digimon.
        /// </summary>
        public int HatchType { get; set; }

        /// <summary>
        /// Section low class data type.
        /// </summary>
        public int LowClassDataSection { get; set; }

        /// <summary>
        /// Section mid class data type.
        /// </summary>
        public int MidClassDataSection { get; set; }

        /// <summary>
        /// Low class data insert amount.
        /// </summary>
        public int LowClassDataAmount { get; set; }

        /// <summary>
        /// Mid class data insert amount.
        /// </summary>
        public int MidClassDataAmount { get; set; }

        public int LowClassLimitLevel { get; set; }
        public int MidClassLimitLevel { get; set; }

        /// <summary>
        /// Low class data insertion break point.
        /// </summary>
        public int LowClassBreakPoint { get; set; }

        /// <summary>
        /// Low class data insertion break point.
        /// </summary>
        public int MidClassBreakPoint { get; set; }

    }
}
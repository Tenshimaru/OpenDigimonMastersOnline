namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class MonthlyEventAssetModel
    {
         /// <summary>
        /// Unique sequential identifier
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Event Current Day For Reward
        /// </summary>
        public int CurrentDay { get; set; }

        /// <summary>
        /// Item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Item Quantity.
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// Detailed skill information.
        /// </summary>
        public MonthlyEventAssetModel MonthlyEventInfo { get; private set; }

      
    }
}
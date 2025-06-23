namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class MonthlyEventAssetDTO
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
    }
}
namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class QuestSupplyAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client item identifier.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Item amount.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Parent object.
        /// </summary>
        public long QuestId { get; set; }
        public QuestAssetModel Quest { get; set; }
    }
}
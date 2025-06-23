namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class QuestConditionAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Quest condition enumeration.
        /// </summary>
        public int ConditionType { get; set; }

        /// <summary>
        /// Quest condition identifier.
        /// </summary>
        public int ConditionId { get; set; }

        /// <summary>
        /// Quest condition amount.
        /// </summary>
        public int ConditionCount { get; set; }

        /// <summary>
        /// Parent object.
        /// </summary>
        public long QuestId { get; set; }
        public QuestAssetModel Quest { get; set; }
    }
}
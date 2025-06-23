namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class QuestEventAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Quest event identifier.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Parent object.
        /// </summary>
        public long QuestId { get; set; }
        public QuestAssetModel Quest { get; set; }
    }
}
namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class QuestEventAssetDTO
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
        public QuestAssetDTO Quest { get; set; }
    }
}
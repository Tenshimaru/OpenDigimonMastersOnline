namespace DigitalWorldOnline.Commons.Models.Chat
{
    public sealed partial class ChatMessageModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Sent date.
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Owner id.
        /// </summary>
        public long CharacterId { get; private set; }

        public ChatMessageModel()
        {
            Time = DateTime.Now;
        }
    }
}
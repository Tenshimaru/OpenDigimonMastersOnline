namespace DigitalWorldOnline.Commons.Models.Chat
{
    public sealed partial class ChatMessageModel
    {
        /// <summary>
        /// Creates a new chat message object.
        /// </summary>
        /// <param name="characterId">Ownner id.</param>
        /// <param name="message">Message.</param>
        public static ChatMessageModel Create(long characterId, string message)
        {
            return new ChatMessageModel()
            {
                CharacterId = characterId,
                Message = message
            };
        }
    }
}
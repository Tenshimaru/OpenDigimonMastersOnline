namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterFriendModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Friend name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Friend annotation.
        /// </summary>
        public string Annotation { get; set; }

        /// <summary>
        /// Connection status.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Friend character id.
        /// </summary>
        public long FriendId { get; private set; }

        public long CharacterId { get; private set; }
        
        public CharacterModel? Character { get; private set; }
        
        public CharacterModel? Friend { get; private set; }

        public static CharacterFriendModel Create(string name, long tamerId, bool online)
        {
            var friend = new CharacterFriendModel()
            {
                Name = name,
                Connected = online,
                FriendId = tamerId
            };

            return friend;
        }

        public void SetTamer(CharacterModel tamer)
        {
            CharacterId = tamer.Id;
        }

        public void SetConnected(bool online)
        {
            Connected = online;
        }
    }
}
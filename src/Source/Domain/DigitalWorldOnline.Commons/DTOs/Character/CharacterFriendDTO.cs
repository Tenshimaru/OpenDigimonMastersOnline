using System.ComponentModel.DataAnnotations;

namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterFriendDTO
    {
        public long Id { get; private set; }
        [MinLength(6)]
        public string Name { get; private set; }
        public string Annotation { get; set; }
        public bool Connected { get; private set; }
        public long FriendId { get; private set; }

        //References
        public long CharacterId { get; private set; }
        public CharacterDTO Character { get; private set; }
        public CharacterDTO Friend { get; private set; }
        
        public void SetConnected(bool connected)
        {
            Connected = connected;
        }
    }
}
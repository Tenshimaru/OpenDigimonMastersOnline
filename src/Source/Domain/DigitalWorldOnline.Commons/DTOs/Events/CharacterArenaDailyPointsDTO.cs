using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Commons.DTOs.Events
{
    public class CharacterArenaDailyPointsDTO
    {
        public Guid Id { get; set; }
        public DateTime InsertDate { get; set; }
        public int Points { get; set; }
 
        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public CharacterDTO Character { get; set; }
        public long CharacterId { get; set; }
    }
}

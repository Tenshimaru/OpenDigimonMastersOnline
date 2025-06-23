using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterBuffDTO : BuffDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long BuffListId { get; set; }
        public CharacterBuffListDTO BuffList { get; set; }
    }
}
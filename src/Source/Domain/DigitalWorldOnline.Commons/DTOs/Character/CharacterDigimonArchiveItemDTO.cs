using DigitalWorldOnline.Commons.DTOs.Digimon;

namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterDigimonArchiveItemDTO
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Archive slot.
        /// </summary>
        public int Slot { get; set; }

        /// <summary>
        /// Digimon reference.
        /// </summary>
        public long DigimonId { get; set; }
        
        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public Guid DigimonArchiveId { get; set; }
        public CharacterDigimonArchiveDTO DigimonArchive { get; set; }
    }
}

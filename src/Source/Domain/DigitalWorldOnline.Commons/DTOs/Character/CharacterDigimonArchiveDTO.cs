namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterDigimonArchiveDTO
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Digimon archives.
        /// </summary>
        public List<CharacterDigimonArchiveItemDTO> DigimonArchives { get; set; }

        /// <summary>
        /// Available archive slots.
        /// </summary>
        public int Slots { get; set; }

        /// <summary>
        /// Reference to character.
        /// </summary>
        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }
    }
}

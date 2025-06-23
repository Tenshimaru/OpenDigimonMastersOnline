namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterBuffListDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Buffs inside the list.
        /// </summary>
        public IList<CharacterBuffDTO> Buffs { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }
    }
}
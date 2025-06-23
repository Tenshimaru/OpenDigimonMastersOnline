namespace DigitalWorldOnline.Commons.DTOs.Digimon
{
    public class DigimonBuffListDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Buffs inside the list.
        /// </summary>
        public List<DigimonBuffDTO> Buffs { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long DigimonId { get; set; }
        public DigimonDTO Digimon { get; set; }
    }
}
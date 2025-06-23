using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Digimon
{
    public class DigimonLocationDTO : LocationDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long DigimonId { get; set; }
        public DigimonDTO Digimon { get; set; }
    }
}

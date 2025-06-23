using DigitalWorldOnline.Commons.Models.Digimon;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public partial class CharacterDigimonArchiveItemModel
    {
        /// <summary>
        /// Set digimon info.
        /// </summary>
        public void SetDigimonInfo(DigimonModel digimon) => Digimon = digimon;

        /// <summary>
        /// Adds a digimon to the target slot.
        /// </summary>
        /// <param name="digimonId">Target digimon identifier</param>
        public void AddDigimon(long digimonId) => DigimonId = digimonId;
        
        /// <summary>
        /// Removes a digimon from the target slot.
        /// </summary>
        public void RemoveDigimon() => DigimonId = 0;
    }
}

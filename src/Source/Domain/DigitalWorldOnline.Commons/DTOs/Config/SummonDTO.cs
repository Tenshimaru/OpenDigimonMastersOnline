using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class SummonDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference for item identifier.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Available maps.
        /// </summary>
        public List<int> Maps { get; set; }

        /// <summary>
        /// Client reference for digimon model.
        /// </summary>
        public List<SummonMobDTO> SummonedMobs { get; set; }
       
    }
}

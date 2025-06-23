using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Commons.ViewModel.Summons
{
    public class SummonViewModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        public ItemAssetViewModel ItemInfo { get; set; }
        public MapConfigViewModel MapConfig { get; set; }

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
        public List<SummonMobViewModel> SummonedMobs { get; set; }
        public SummonViewModel() 
        { 
            Maps = new List<int>();
            SummonedMobs = new List<SummonMobViewModel>();
        }    
    }
}
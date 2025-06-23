using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Commons.Models.Summon
{
    public class SummonModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        public ItemAssetViewModel ItemInfo { get; set; }

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
        public List<SummonMobModel> SummonedMobs { get; set; }
        public SummonModel()
        {
            Maps = new List<int>();
            SummonedMobs = new List<SummonMobModel>();
        }
    }

}

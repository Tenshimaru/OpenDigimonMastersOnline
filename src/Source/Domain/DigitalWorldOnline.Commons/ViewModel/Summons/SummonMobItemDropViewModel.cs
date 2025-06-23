using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Commons.ViewModel.Summons
{
    public class SummonMobItemDropViewModel
    {
        /// <summary>
        /// JIT property.
        /// </summary>
        public ItemAssetViewModel ItemInfo { get; set; }

        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client item id information.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Min. amount of the item.
        /// </summary>
        public int MinAmount { get; set; }

        /// <summary>
        /// Max. amount of the item.
        /// </summary>
        public int MaxAmount { get; set; }

        /// <summary>
        /// Chance of drop.
        /// </summary>
        public double Chance { get; set; }

        public SummonMobItemDropViewModel()
        {
            MinAmount = 1;
            MaxAmount = 1;
        }

        public SummonMobItemDropViewModel(long lastId)
        {
            Id = lastId + 1;
            MinAmount = 1;
            MaxAmount = 1;
        }
    }
}

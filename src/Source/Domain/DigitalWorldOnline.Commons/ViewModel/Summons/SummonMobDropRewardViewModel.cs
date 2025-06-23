using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Commons.ViewModel.Summons
{
    public class SummonMobDropRewardViewModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Min. amount of drops.
        /// </summary>
        public byte MinAmount { get; set; }

        /// <summary>
        /// Max. amount of drops.
        /// </summary>
        public byte MaxAmount { get; set; }

        /// <summary>
        /// Item drop list
        /// </summary>
        public List<SummonMobItemDropViewModel> Drops { get; set; }

        /// <summary>
        /// Bits drop config
        /// </summary>
        public SummonMobBitDropViewModel BitsDrop { get; set; }

        public SummonMobDropRewardViewModel()
        {
            MinAmount = 0;
            MaxAmount = 1;
            Drops = new List<SummonMobItemDropViewModel>();
            BitsDrop = new SummonMobBitDropViewModel();
        }
    }
}

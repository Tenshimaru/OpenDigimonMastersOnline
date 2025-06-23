using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Commons.ViewModel.Mobs
{
    public class MobDropRewardViewModel
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
        public List<MobItemDropViewModel> Drops { get; set; }

        /// <summary>
        /// Bits drop config
        /// </summary>
        public MobBitDropViewModel BitsDrop { get; set; }

        public MobDropRewardViewModel()
        {
            MinAmount = 0;
            MaxAmount = 1;
            Drops = new List<MobItemDropViewModel>();
            BitsDrop = new MobBitDropViewModel();
        }
    }
}

namespace DigitalWorldOnline.Commons.ViewModel
{
    public class MobDropRewardConfigViewModel
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
        public IList<ItemDropConfigViewModel> Drops { get; set; }

        /// <summary>
        /// Bits drop config
        /// </summary>
        public BitsDropConfigViewModel BitsDrop { get; set; }

        //TODO: Behavior
        public MobDropRewardConfigViewModel()
        {
            BitsDrop = new BitsDropConfigViewModel();
            Drops = new List<ItemDropConfigViewModel>();
            MinAmount = 0;
            MaxAmount = 0;
        }
    }
}

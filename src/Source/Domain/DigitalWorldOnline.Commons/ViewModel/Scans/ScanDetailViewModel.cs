using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Commons.ViewModel.Scans
{
    public class ScanDetailViewModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Min. amount of rewards.
        /// </summary>
        public byte MinAmount { get; set; }

        /// <summary>
        /// Max. amount of rewards.
        /// </summary>
        public byte MaxAmount { get; set; }

        /// <summary>
        /// Client reference to the item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Referenced item name.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Available rewards.
        /// </summary>
        public LinkedList<ScanRewardDetailViewModel> Rewards { get; set; }

        /// <summary>
        /// JIT property.
        /// </summary>
        public ItemAssetViewModel ItemInfo { get; set; }

        /// <summary>
        /// Flag for invalid create configuration.
        /// </summary>
        public bool Invalid => ItemInfo == null || MinAmount > MaxAmount || MinAmount > 0 && !Rewards.Any();

        public ScanDetailViewModel()
        {
            MaxAmount = 1;
            Rewards = new LinkedList<ScanRewardDetailViewModel>();
        }
    }
}

using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Commons.ViewModel.Containers
{
    public class ContainerViewModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Min. amount of rewards.
        /// </summary>
        public int RewardAmount { get; set; }

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
        public LinkedList<ContainerRewardViewModel> Rewards { get; set; }

        /// <summary>
        /// JIT property.
        /// </summary>
        public ItemAssetViewModel ItemInfo { get; set; }

        /// <summary>
        /// Flag for invalid create configuration.
        /// </summary>
        public bool Invalid => ItemInfo == null || RewardAmount == 0 || RewardAmount > 0 && !Rewards.Any();

        public ContainerViewModel()
        {
            RewardAmount = 1;
            Rewards = new LinkedList<ContainerRewardViewModel>();
        }
    }
}

using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Commons.ViewModel.Scans
{
    public class ScanRewardDetailViewModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference to the item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Refered item name.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Minimal reward amount.
        /// </summary>
        public int MinAmount { get; set; }

        /// <summary>
        /// Maximum reward amount.
        /// </summary>
        public int MaxAmount { get; set; }

        /// <summary>
        /// Reward aquire chance.
        /// </summary>
        public double Chance { get; set; }

        /// <summary>
        /// Flag for global message display.
        /// </summary>
        public bool Rare { get; set; }

        /// <summary>
        /// JIT property.
        /// </summary>
        public ItemAssetViewModel ItemInfo { get; set; }
        
        /// <summary>
        /// Flag for invalid reward configuration.
        /// </summary>
        public bool Invalid => MinAmount > MaxAmount || ItemInfo == null;

        public ScanRewardDetailViewModel() 
        {
            MinAmount = 1;
            MaxAmount = 1;
        }

        public static ScanRewardDetailViewModel Create(long id)
        {
            return new ScanRewardDetailViewModel()
            {
                Id = id + 1
            };
        }
    }
}
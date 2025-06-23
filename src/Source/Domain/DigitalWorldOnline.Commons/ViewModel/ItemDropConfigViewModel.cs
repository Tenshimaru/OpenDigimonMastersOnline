namespace DigitalWorldOnline.Commons.ViewModel
{
    public class ItemDropConfigViewModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client item id information.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Item name.
        /// </summary>
        public string Name { get; set; }

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

        /// <summary>
        /// Reference to the drop reward.
        /// </summary>
        public long DropRewardId { get; set; }

        public ItemDropConfigViewModel()
        {
            ItemId = 90101;
            Chance = 50;
            MinAmount = 1;
            MaxAmount = 1;
        }
    }
}
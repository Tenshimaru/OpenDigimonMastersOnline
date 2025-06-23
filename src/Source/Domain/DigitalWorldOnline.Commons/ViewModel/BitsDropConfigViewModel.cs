namespace DigitalWorldOnline.Commons.ViewModel
{
    public class BitsDropConfigViewModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

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

        public BitsDropConfigViewModel()
        {
            MinAmount = 50;
            MaxAmount = 100;
            Chance = 50;
        }
    }
}
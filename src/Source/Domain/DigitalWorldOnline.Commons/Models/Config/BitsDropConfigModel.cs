namespace DigitalWorldOnline.Commons.Models.Config
{
    public class BitsDropConfigModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Min. amount of the item.
        /// </summary>
        public int MinAmount { get; private set; }
        
        /// <summary>
        /// Max. amount of the item.
        /// </summary>
        public int MaxAmount { get; private set; }

        /// <summary>
        /// Chance of drop.
        /// </summary>
        public double Chance { get; private set; }

        /// <summary>
        /// Reference to the drop reward.
        /// </summary>
        public long DropRewardId { get; private set; }

        public BitsDropConfigModel()
        {
            MinAmount = 50;
            MaxAmount = 100;
            Chance = 50;
        }
    }
}
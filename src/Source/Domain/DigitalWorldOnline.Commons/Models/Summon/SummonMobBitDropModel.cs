
namespace DigitalWorldOnline.Commons.Models.Summon
{
    public class SummonMobBitDropModel
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

        public SummonMobBitDropModel()
        {
            MinAmount = 0;
            MaxAmount = 150;
            Chance = 87.5;
        }
    }
}

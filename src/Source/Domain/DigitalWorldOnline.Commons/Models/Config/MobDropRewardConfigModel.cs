namespace DigitalWorldOnline.Commons.Models.Config
{
    public class MobDropRewardConfigModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Min. amount of drops.
        /// </summary>
        public byte MinAmount { get; private set; }

        /// <summary>
        /// Max. amount of drops.
        /// </summary>
        public byte MaxAmount { get; private set; }

        /// <summary>
        /// Item drop list
        /// </summary>
        public List<ItemDropConfigModel> Drops { get; private set; }

        /// <summary>
        /// Bits drop config
        /// </summary>
        public BitsDropConfigModel BitsDrop { get; private set; }

        public MobDropRewardConfigModel()
        {
            BitsDrop = new BitsDropConfigModel();
            Drops = new List<ItemDropConfigModel>();
            MinAmount = 1;
            MaxAmount = 1;
        }
    }
}

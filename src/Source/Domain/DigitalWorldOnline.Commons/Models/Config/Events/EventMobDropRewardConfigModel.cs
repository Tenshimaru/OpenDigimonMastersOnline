namespace DigitalWorldOnline.Commons.Models.Config.Events
{
    public class EventMobDropRewardConfigModel
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
        public List<EventItemDropConfigModel> Drops { get; private set; }

        /// <summary>
        /// Bits drop config
        /// </summary>
        public EventBitsDropConfigModel BitsDrop { get; private set; }

        public EventMobDropRewardConfigModel()
        {
            BitsDrop = new EventBitsDropConfigModel();
            Drops = new List<EventItemDropConfigModel>();
            MinAmount = 1;
            MaxAmount = 1;
        }
    }
}
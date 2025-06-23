namespace DigitalWorldOnline.Commons.DTOs.Config.Events
{
    public class EventMobDropRewardConfigDTO
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
        public List<EventItemDropConfigDTO> Drops { get; set; }

        /// <summary>
        /// Bits drop config
        /// </summary>
        public EventBitsDropConfigDTO BitsDrop { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long MobId { get; set; }

        public EventMobConfigDTO Mob { get; set; }

        public EventMobDropRewardConfigDTO()
        {
            Drops = new List<EventItemDropConfigDTO>();
            BitsDrop = new EventBitsDropConfigDTO();
        }
    }
}
namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class MobDropRewardConfigDTO
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
        public List<ItemDropConfigDTO> Drops { get; set; }

        /// <summary>
        /// Bits drop config
        /// </summary>
        public BitsDropConfigDTO BitsDrop { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long MobId { get; set; }
        public MobConfigDTO Mob { get; set; }

        public MobDropRewardConfigDTO()
        {
            Drops = new List<ItemDropConfigDTO>();
            BitsDrop = new BitsDropConfigDTO();
        }
    }
}

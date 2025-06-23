
namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class SummonMobDropRewardDTO
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
        public List<SummonMobItemDropDTO> Drops { get; set; }

        /// <summary>
        /// Bits drop config
        /// </summary>
        public SummonMobBitDropDTO BitsDrop { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long MobId { get; set; }
        public SummonMobDTO Mob { get; set; }
    }
}

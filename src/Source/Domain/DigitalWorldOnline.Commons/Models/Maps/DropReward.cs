using DigitalWorldOnline.Commons.Models.Base;

namespace DigitalWorldOnline.Commons.Models.Maps
{
    public class DropReward
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Item drop list
        /// </summary>
        public IList<ItemDrop> Drops { get; set; }

        /// <summary>
        /// Reference to the target mob.
        /// </summary>
        public long MobId { get; set; }
    }
}

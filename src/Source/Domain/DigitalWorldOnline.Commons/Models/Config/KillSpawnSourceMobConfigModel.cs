

namespace DigitalWorldOnline.Commons.Models.Config
{
    public sealed partial class KillSpawnSourceMobConfigModel
    {
        public long Id { get; private set; }

        /// <summary>
        /// Source mob identifier.
        /// </summary>
        public int SourceMobType { get; private set; }

        /// <summary>
        /// Source mob kill amount.
        /// </summary>
        public byte SourceMobRequiredAmount { get; private set; }

        /// <summary>
        /// Current source mob kill amount.
        /// </summary>
        public byte CurrentSourceMobRequiredAmount { get; private set; }
    }
}

using DigitalWorldOnline.Commons.Models.Asset;

namespace DigitalWorldOnline.Commons.Models
{
    public partial class Buff
    {
        /// <summary>
        /// Client reference for the target buff.
        /// </summary>
        public int BuffId { get; private set; }

        /// <summary>
        /// Total duration in seconds.
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// Client reference for the target skill id.
        /// </summary>
        public int SkillId { get; private set; }

        public int TypeN { get; private set; }

        public int Cooldown { get; private set; }

        /// <summary>
        /// Time when the buff expires
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Time when the buff Cooldown expires
        /// </summary>
        public DateTime CoolEndDate { get; private set; }


        /// <summary>
        /// Buff details.
        /// </summary>
        public BuffInfoAssetModel? BuffInfo { get; private set; }
    }
}
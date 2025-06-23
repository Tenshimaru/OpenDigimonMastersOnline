using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Events
{
    public sealed partial class TimeRewardModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long CharacterId { get; private set; }

        /// <summary>
        /// The current index start time.
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// The reward current index and duration.
        /// </summary>
        public TimeRewardIndexEnum RewardIndex { get; set; }

        /// <summary>
        /// The current time.
        /// </summary>
        public int AtualTime { get; private set; }

        public int UpdateCounter { get; set; } = 0;
    }
}
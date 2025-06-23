namespace DigitalWorldOnline.Commons.Models.Events
{
    public sealed partial class AttendanceRewardModel
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
        /// The current log-in streak.
        /// </summary>
        public byte TotalDays { get; private set; }

        /// <summary>
        /// The date when the last reward was claimed.
        /// </summary>
        public DateTime LastRewardDate { get; private set; }
    }
}
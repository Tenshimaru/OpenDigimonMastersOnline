using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Commons.DTOs.Events
{
    public class TimeRewardDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The reward current index and duration.
        /// </summary>
        public TimeRewardIndexEnum RewardIndex { get; set; }

        /// <summary>
        /// The current index start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }

        /// <summary>
        /// The current time.
        /// </summary>
        public int AtualTime { get; set; }
    }
}

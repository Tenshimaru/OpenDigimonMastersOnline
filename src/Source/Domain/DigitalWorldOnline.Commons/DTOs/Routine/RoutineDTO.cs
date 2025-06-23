using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Routine
{
    public sealed class RoutineDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Routine enumeration.
        /// </summary>
        public RoutineTypeEnum Type { get; set; }

        /// <summary>
        /// Routine active status.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Routine description.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Routine interval (in days).
        /// </summary>
        public int Interval { get; set; }
        
        /// <summary>
        /// Routine next run time.
        /// </summary>
        public DateTime NextRunTime { get; set; }

        /// <summary>
        /// Routine create date.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
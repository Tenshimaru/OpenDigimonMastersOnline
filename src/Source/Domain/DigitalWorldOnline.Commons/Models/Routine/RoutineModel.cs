using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Models.DTOs.Routine
{
    public sealed partial class RoutineModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Routine enumeration.
        /// </summary>
        public RoutineTypeEnum Type { get; private set; }
        
        /// <summary>
        /// Routine active status.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Routine description.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Routine interval (in days).
        /// </summary>
        public int Interval { get; private set; }
        
        /// <summary>
        /// Routine next run time.
        /// </summary>
        public DateTime NextRunTime { get; private set; }

        /// <summary>
        /// Routine create date.
        /// </summary>
        public DateTime CreatedAt { get; private set; }
    }
}
using DigitalWorldOnline.Commons.Enums.Events;

namespace DigitalWorldOnline.Commons.DTOs.Config.Events
{
    public sealed class EventConfigDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Map name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Monster coliseum Round
        /// </summary>
        public byte Rounds { get; set; }

        /// <summary>
        /// Indicates if the map is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Specifies the day of the week on which the event starts.
        /// </summary>
        public EventStartDayEnum StartDay { get; set; } = EventStartDayEnum.Everyday;
        
        /// <summary>
        /// Duration of time from the start.
        /// </summary>
        public TimeSpan StartsAt { get; set; }

        /// <summary>
        /// Child mobs.
        /// </summary>
        public List<EventMapsConfigDTO> EventMaps { get; set; }
    }
}
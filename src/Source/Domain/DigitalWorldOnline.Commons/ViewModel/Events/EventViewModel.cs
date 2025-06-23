using DigitalWorldOnline.Commons.Enums.Events;
using DigitalWorldOnline.Commons.Models.Config.Events;

namespace DigitalWorldOnline.Commons.ViewModel.Events
{
    public class EventViewModel
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
        /// Brief textual summary providing details about the event.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates whether the event is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Monster coliseum Round
        /// </summary>
        public byte Rounds { get; set; }

        public EventStartDayEnum StartDay { get; set; } = EventStartDayEnum.Everyday;
        public TimeSpan? StartsAt { get; set; } = new TimeSpan(00, 00, 00);
        public List<EventMapViewModel> EventMaps { get; set; }
    }
}
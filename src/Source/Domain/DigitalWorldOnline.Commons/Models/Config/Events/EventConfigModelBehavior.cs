using DigitalWorldOnline.Commons.Enums.Events;

namespace DigitalWorldOnline.Commons.Models.Config.Events
{
    public sealed partial class EventConfigModel
    {
        public void SetStartsAt(TimeSpan startsAt) => StartsAt = startsAt;

        public void SetStartDay(EventStartDayEnum startDay) => StartDay = startDay;
    }
}
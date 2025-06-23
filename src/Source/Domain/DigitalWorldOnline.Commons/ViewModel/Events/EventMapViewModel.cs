using DigitalWorldOnline.Commons.ViewModel.Maps;

namespace DigitalWorldOnline.Commons.ViewModel.Events
{
    public class EventMapViewModel
    {
        public long Id { get; set; }
        public long EventConfigId { get; set; }
        public int MapId { get; set; }
        
        public int Channels { get; set; }
        
        public bool IsEnabled { get; set; }
        public MapViewModel Map { get; set; }
        
        public EventViewModel EventConfig { get; set; }

        /// <summary>
        /// Map mobs count.
        /// </summary>
        public int MobsCount { get; set; }
    }
}

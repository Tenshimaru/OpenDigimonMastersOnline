namespace DigitalWorldOnline.Commons.Models.Config.Events
{
    public sealed class EventMapsConfigModel
    {
        public long Id { get; set; }
        public long EventConfigId { get; set; }
        public int MapId { get; set; }
        public int Channels { get; set; }
        public bool IsEnabled { get; set; }
        public MapConfigModel Map { get; set; }
        public EventConfigModel EventConfig { get; set; }

        public EventMapsConfigModel(long eventConfigId, int mapId, int channels, MapConfigModel map,
            bool isEnabled = true)
        {
            EventConfigId = eventConfigId;
            MapId = mapId;
            Channels = channels;
            IsEnabled = isEnabled;
            Map = map;
        }
    }
}
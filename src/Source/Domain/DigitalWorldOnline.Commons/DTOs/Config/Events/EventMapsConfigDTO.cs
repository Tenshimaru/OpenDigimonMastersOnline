namespace DigitalWorldOnline.Commons.DTOs.Config.Events
{
    public sealed class EventMapsConfigDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long EventConfigId { get; set; }

        /// <summary>
        /// Map id reference to target map.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Channels reference to target map.
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// Indicates if the map is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Child map.
        /// </summary>
        public MapConfigDTO Map { get; set; }

        /// <summary>
        /// Child map.
        /// </summary>
        public EventConfigDTO EventConfig { get; set; }

        public List<EventMobConfigDTO> Mobs { get; set; }
    }
}
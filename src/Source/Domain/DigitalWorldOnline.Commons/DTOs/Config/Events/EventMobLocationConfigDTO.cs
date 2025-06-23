using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Config.Events
{
    public class EventMobLocationConfigDTO : LocationDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long MobConfigId { get; set; }

        public EventMobConfigDTO MobConfig { get; set; }
    }
}
namespace DigitalWorldOnline.Commons.Models.Config.Events
{
    public sealed partial class EventMobLocationConfigModel : Location
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }
    }
}
namespace DigitalWorldOnline.Commons.Models.Config
{
    public sealed partial class MobLocationConfigModel : Location
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }
    }
}
namespace DigitalWorldOnline.Commons.ViewModel.Maps
{
    public class MapViewModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client id reference to target map.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Map name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Map mobs count.
        /// </summary>
        public int MobsCount { get; set; }
    }
}

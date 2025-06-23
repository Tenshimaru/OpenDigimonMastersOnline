namespace DigitalWorldOnline.Commons.Models
{
    public partial class Location
    {
        /// <summary>
        /// Reference ID to map.
        /// </summary>
        public short MapId { get; private set; }

        /// <summary>
        /// Position X (horizontal).
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Position Y (vertical).
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Position Z (looking for).
        /// </summary>
        public float Z { get; private set; }

        public int TicksCount { get; private set; }
    }
}
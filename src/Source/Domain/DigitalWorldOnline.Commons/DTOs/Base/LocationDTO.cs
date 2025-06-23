namespace DigitalWorldOnline.Commons.DTOs.Base
{
    public class LocationDTO
    {
        /// <summary>
        /// Reference ID to map.
        /// </summary>
        public short MapId { get; set; }

        /// <summary>
        /// Position X.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Position Y.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Position Z (looking for).
        /// </summary>
        public float Z { get; set; }
    }
}

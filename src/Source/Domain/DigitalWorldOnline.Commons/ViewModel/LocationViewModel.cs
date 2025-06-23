namespace DigitalWorldOnline.Commons.ViewModel
{
    public class LocationViewModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference ID to map.
        /// </summary>
        public short MapId { get; set; }

        /// <summary>
        /// Position X (horizontal).
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Position Y (vertical).
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Position Z (looking for).
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Reference to the target owner.
        /// </summary>
        public long OwnerId { get; set; }
    }
}

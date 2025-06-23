namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public sealed class HashDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The hash itself.
        /// </summary>
        public string Hash{ get; set; }

        /// <summary>
        /// Hash create date.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
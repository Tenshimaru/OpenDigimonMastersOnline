namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public sealed class WelcomeMessageConfigDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The message itself.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Flag to use or not.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
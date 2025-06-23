using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Mechanics
{
    public class GuildAuthorityDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Authority type enumeration.
        /// </summary>
        public GuildAuthorityTypeEnum Class { get; set; }

        /// <summary>
        /// Authority title description.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Authority duty description.
        /// </summary>
        public string Duty { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public GuildDTO Guild { get; set; }
        public long GuildId { get; set; }
    }
}
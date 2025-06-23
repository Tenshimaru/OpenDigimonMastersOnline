using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public sealed partial class GuildAuthorityModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Authority type enumeration.
        /// </summary>
        public GuildAuthorityTypeEnum Class { get; private set; }

        /// <summary>
        /// Authority title description.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Authority duty description.
        /// </summary>
        public string Duty { get; private set; }
    }
}
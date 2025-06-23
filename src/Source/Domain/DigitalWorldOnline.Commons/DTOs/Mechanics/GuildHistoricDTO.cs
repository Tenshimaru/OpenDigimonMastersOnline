using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Mechanics
{
    public class GuildHistoricDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Historic type enumeration.
        /// </summary>
        public GuildHistoricTypeEnum Type { get; set; }

        /// <summary>
        /// Historic entry date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Guild master authority enumeration.
        /// </summary>
        public GuildAuthorityTypeEnum MasterClass { get; set; }

        /// <summary>
        /// Guild master current name.
        /// </summary>
        public string MasterName { get; set; }

        /// <summary>
        /// Guild member authority enumeration.
        /// </summary>
        public GuildAuthorityTypeEnum MemberClass { get; set; }

        /// <summary>
        /// Guild member current name.
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public GuildDTO Guild { get; set; }
        public long GuildId { get; set; }
    }
}
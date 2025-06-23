using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public sealed partial class GuildHistoricModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Historic type enumeration.
        /// </summary>
        public GuildHistoricTypeEnum Type { get; private set; }

        /// <summary>
        /// Historic entry date.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Guild master authority enumeration.
        /// </summary>
        public GuildAuthorityTypeEnum MasterClass { get; private set; }

        /// <summary>
        /// Guild master current name.
        /// </summary>
        public string MasterName { get; private set; }

        /// <summary>
        /// Guild member authority enumeration.
        /// </summary>
        public GuildAuthorityTypeEnum MemberClass { get; private set; }

        /// <summary>
        /// Guild member current name.
        /// </summary>
        public string MemberName { get; private set; }
    }
}
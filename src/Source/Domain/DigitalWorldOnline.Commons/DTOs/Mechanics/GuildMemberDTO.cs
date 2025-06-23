using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Commons.DTOs.Mechanics
{
    public class GuildMemberDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Member total contribution points.
        /// </summary>
        public int Contribution { get; set; }

        /// <summary>
        /// Member authority information.
        /// </summary>
        public GuildAuthorityTypeEnum Authority { get; set; }

        /// <summary>
        /// Tamer member reference.
        /// </summary>
        public CharacterDTO Character { get; set; }
        public long CharacterId { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public GuildDTO Guild { get; set; }
        public long GuildId { get; set; }
    }
}
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Models.Character;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public sealed partial class GuildMemberModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Member authority enumeration.
        /// </summary>
        public GuildAuthorityTypeEnum Authority { get; private set; }

        /// <summary>
        /// Member total contribution points.
        /// </summary>
        public int Contribution { get; private set; }

        /// <summary>
        /// Reference to the target tamer.
        /// </summary>
        public long CharacterId { get; private set; }

        /// <summary>
        /// Reference to the target tamer.
        /// </summary>
        public CharacterModel CharacterInfo { get; private set; }
    }
}

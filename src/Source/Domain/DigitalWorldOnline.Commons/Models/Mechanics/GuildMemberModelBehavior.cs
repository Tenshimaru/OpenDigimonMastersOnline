using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Models.Character;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public sealed partial class GuildMemberModel
    {
        /// <summary>
        /// Creates a new guild member object.
        /// </summary>
        /// <param name="character">Tamer member info</param>
        /// <param name="memberClass">Member authority</param>
        public static GuildMemberModel Create(CharacterModel character, GuildAuthorityTypeEnum memberClass)
        {
            return new GuildMemberModel()
            {
                CharacterInfo = character,
                CharacterId = character.Id,
                Contribution = 0,
                Authority = memberClass
            };
        }

        /// <summary>
        /// Returns member tamer model explicit value.
        /// </summary>
        public byte MemberModel => (byte)(CharacterInfo.Model - 80000);

        /// <summary>
        /// Updates the character information for the target member.
        /// </summary>
        /// <param name="character">Character information</param>
        public void SetCharacterInfo(CharacterModel character) => CharacterInfo = character;

        /// <summary>
        /// Updates the member current authority type.
        /// </summary>
        /// <param name="newAuthority">New authority type</param>
        public void SetAuthority(GuildAuthorityTypeEnum newAuthority) => Authority = newAuthority;
    }
}

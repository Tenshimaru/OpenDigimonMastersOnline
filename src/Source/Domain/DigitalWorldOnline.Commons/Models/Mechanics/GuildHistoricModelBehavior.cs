using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public sealed partial class GuildHistoricModel
    {
        /// <summary>
        /// Creates a new guild historic entry object.
        /// </summary>
        /// <param name="type">Entry type</param>
        /// <param name="leader">Guild master information</param>
        /// <param name="member">Target member information</param>
        public static GuildHistoricModel Create(GuildHistoricTypeEnum type, GuildMemberModel leader, GuildMemberModel member)
        {
            return new GuildHistoricModel()
            {
                Date = DateTime.Now,
                Type = type,
                MasterClass = leader.Authority,
                MasterName = leader.CharacterInfo.Name,
                MemberClass = member.Authority,
                MemberName = member.CharacterInfo.Name
            };
        }
    }
}
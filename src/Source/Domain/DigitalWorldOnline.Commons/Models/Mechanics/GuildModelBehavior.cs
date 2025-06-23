using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Character;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public sealed partial class GuildModel
    {
        /// <summary>
        /// Creates a new guild object.
        /// </summary>
        /// <param name="name">Guild name</param>
        /// <param name="notice">Guild notice</param>
        /// <param name="level">Guild current level</param>
        /// <param name="extraSlots">Guild current extra member slots</param>
        public static GuildModel Create(string name,
            string notice = "Welcome to the guild!!!",
            byte level = 1,
            byte extraSlots = byte.MinValue,
            int currentExperience = 0)
        {
            var baseAuthorities = new List<GuildAuthorityModel>()
            {
                GuildAuthorityModel.Create(GuildAuthorityTypeEnum.Master),
                GuildAuthorityModel.Create(GuildAuthorityTypeEnum.SubMaster),
                GuildAuthorityModel.Create(GuildAuthorityTypeEnum.DatsMember),
                GuildAuthorityModel.Create(GuildAuthorityTypeEnum.Member),
                GuildAuthorityModel.Create(GuildAuthorityTypeEnum.NewMember)
            };

            return new GuildModel()
            {
                Members = new List<GuildMemberModel>(),
                Skills = new List<GuildSkillModel>(),
                Historic = new List<GuildHistoricModel>(),
                Authority = baseAuthorities,
                CreationDate = DateTime.Now,
                Name = name,
                Notice = notice,
                Level = level,
                ExtraSlots = extraSlots,
                CurrentExperience = currentExperience
            };
        }

        /// <summary>
        /// Returns the guild master information.
        /// </summary>
        public GuildMemberModel Master => Members.First(x => x.Authority == GuildAuthorityTypeEnum.Master);

        /// <summary>
        /// Gets guild members character id.
        /// </summary>
        public List<long> GetGuildMembersIdList() => Members.Select(x => x.CharacterId).ToList();

        /// <summary>
        /// Adds a new member to the guild.
        /// </summary>
        /// <param name="character">Member tamer</param>
        /// <param name="memberClass">Member class</param>
        public GuildMemberModel AddMember(CharacterModel character, GuildAuthorityTypeEnum memberClass = GuildAuthorityTypeEnum.NewMember)
        {
            var member = GuildMemberModel.Create(character, memberClass);

            Members.Add(member);

            return member;
        }
        
        /// <summary>
        /// Returns the first guild member with the target character id.
        /// </summary>
        /// <param name="characterId">Character identifier</param>
        public GuildMemberModel? FindMember(long characterId)
        {
            return Members.FirstOrDefault(x => x.CharacterId == characterId);
        }
        
        /// <summary>
        /// Removes a member from the guild.
        /// </summary>
        /// <param name="characterId">Target member character id</param>
        public void RemoveMember(long characterId)
        {
            Members.RemoveAll(x => x.CharacterId == characterId);
        }
        
        /// <summary>
        /// Returns the first guild authority information for the target type.
        /// </summary>
        /// <param name="authorityType">Target type</param>
        public GuildAuthorityModel FindAuthority(GuildAuthorityTypeEnum authorityType)
        {
            return Authority.First(x=>x.Class == authorityType);
        }

        /// <summary>
        /// Adds a new entry to the guild historic.
        /// </summary>
        /// <param name="leader">Entry leader information</param>
        /// <param name="member">Entry member information</param>
        /// <param name="type">Entry type</param>
        public GuildHistoricModel? AddHistoricEntry(GuildHistoricTypeEnum type, GuildMemberModel? leader, GuildMemberModel? member)
        {
            if (leader == null || member == null)
                return null;

            var entry = GuildHistoricModel.Create(type, leader, member);

            Historic.Add(entry);

            return entry;
        }

        /// <summary>
        /// Updates the current guild message.
        /// </summary>
        /// <param name="newMessage">The new message</param>
        public void SetNotice(string newMessage) => Notice = newMessage;

        /// <summary>
        /// Updates the target authority title and duty.
        /// </summary>
        /// <param name="type">Authority enumeration</param>
        /// <param name="newTitle">New title</param>
        /// <param name="newDuty">New duty</param>
        public GuildAuthorityModel UpdateAuthorityTitle(GuildAuthorityTypeEnum type, string newTitle, string newDuty)
        {
            return Authority.First(x => x.Class == type).Update(newTitle, newDuty);
        }
    }
}
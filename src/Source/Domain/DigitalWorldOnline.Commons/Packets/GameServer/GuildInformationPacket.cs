using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildInformationPacket : PacketWriter
    {
        private const int PacketNumber = 2113;

        /// <summary>
        /// Updates the character guild information.
        /// </summary>
        /// <param name="guild">Guild information</param>
        public GuildInformationPacket(GuildModel guild)
        {
            Type(PacketNumber);
            WriteString(guild.Name);
            WriteInt((int)guild.Id);
            WriteByte(guild.Level);
            WriteInt(guild.CurrentExperience);
            WriteString(guild.Notice);
            WriteInt(guild.ExtraSlots);

            foreach (var authority in guild.Authority)
            {
                WriteString(authority.Title);
                WriteString(authority.Duty);
            }

            var memberList = guild.Members
                .OrderByDescending(x => x.CharacterInfo.Location.MapId)
                //.OrderByDescending(x => x.CharacterInfo.Channel)
                .ThenBy(x => x.Authority.GetHashCode());

            foreach (var member in memberList)
            {
                WriteByte((byte)member.Authority);
                WriteByte(member.MemberModel);
                WriteString(member.CharacterInfo.Name);
                WriteInt(member.Contribution);
                WriteByte(member.CharacterInfo.Level);

                if (member.CharacterInfo.State is CharacterStateEnum.Connected or CharacterStateEnum.Ready)
                {
                    WriteShort(member.CharacterInfo.Location.MapId);
                    WriteByte(member.CharacterInfo.Channel);
                }
                else
                {
                    WriteShort(0);
                }
            }

            WriteByte(0);
        }
    }
}
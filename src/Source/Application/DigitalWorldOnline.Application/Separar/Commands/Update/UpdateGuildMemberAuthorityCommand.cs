using DigitalWorldOnline.Commons.Models.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateGuildMemberAuthorityCommand : IRequest
    {
        public GuildMemberModel GuildMember { get; private set; }

        public UpdateGuildMemberAuthorityCommand(GuildMemberModel guildMember)
        {
            GuildMember = guildMember;
        }
    }
}
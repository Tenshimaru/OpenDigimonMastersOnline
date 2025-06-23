using DigitalWorldOnline.Commons.Models.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateGuildMemberCommand : IRequest
    {
        public GuildMemberModel? Member { get; private set; }
        public long GuildId { get; private set; }

        public CreateGuildMemberCommand(GuildMemberModel? member, long guildId)
        {
            Member = member;
            GuildId = guildId;
        }
    }
}
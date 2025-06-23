using DigitalWorldOnline.Commons.Models.Digimon;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateGuildNoticeCommand : IRequest
    {
        public long GuildId { get; private set; }
        public string NewMessage { get; private set; }

        public UpdateGuildNoticeCommand(long guildId, string newMessage)
        {
            GuildId = guildId;
            NewMessage = newMessage;
        }
    }
}
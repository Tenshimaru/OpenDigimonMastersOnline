using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateGuildCommand : IRequest<GuildDTO>
    {
        public GuildModel Guild { get; set; }

        public CreateGuildCommand(GuildModel guild)
        {
            Guild = guild;
        }
    }
}
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateGuildAuthorityCommand : IRequest
    {
        public GuildAuthorityModel Authority { get; private set; }

        public UpdateGuildAuthorityCommand(GuildAuthorityModel authority)
        {
            Authority = authority;
        }
    }
}
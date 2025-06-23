using MediatR;
using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.Enums.Account;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ServersQuery : IRequest<IList<ServerDTO>>
    {
        public AccountAccessLevelEnum AccessLevel { get; }

        public ServersQuery(AccountAccessLevelEnum accessLevel)
        {
            AccessLevel = accessLevel;
        }
    }
}


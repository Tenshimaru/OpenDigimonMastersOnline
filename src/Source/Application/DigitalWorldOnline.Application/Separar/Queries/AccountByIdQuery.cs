using MediatR;
using DigitalWorldOnline.Commons.DTOs.Account;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AccountByIdQuery : IRequest<AccountDTO?>
    {
        public long Id { get; set; }

        public AccountByIdQuery(long id)
        {
            Id = id;
        }
    }
}


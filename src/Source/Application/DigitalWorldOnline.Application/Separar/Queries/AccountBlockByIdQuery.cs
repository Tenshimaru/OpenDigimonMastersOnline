using MediatR;
using DigitalWorldOnline.Commons.DTOs.Account;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AccountBlockByIdQuery : IRequest<AccountBlockDTO?>
    {
        public long Id { get; set; }

        public AccountBlockByIdQuery(long id)
        {
            Id = id;
        }
    }
}


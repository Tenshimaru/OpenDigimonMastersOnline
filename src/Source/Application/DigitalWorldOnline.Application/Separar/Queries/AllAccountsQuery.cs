using MediatR;
using DigitalWorldOnline.Commons.DTOs.Account;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AllAccountsQuery : IRequest<IList<AccountDTO>>
    {
    }
}
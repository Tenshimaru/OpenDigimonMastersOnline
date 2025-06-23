using MediatR;
using DigitalWorldOnline.Commons.DTOs.Account;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class StaffAccountsQuery : IRequest<IList<AccountDTO>>
    {
    }
}
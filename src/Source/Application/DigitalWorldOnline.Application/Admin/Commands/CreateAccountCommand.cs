using DigitalWorldOnline.Commons.DTOs.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateAccountCommand : IRequest<AccountDTO>
    {
        public AccountDTO Account { get; }

        public CreateAccountCommand(AccountDTO account)
        {
            Account = account;
        }
    }
}
using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Models.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateAccountCommand : IRequest
    {
        public AccountModel Account { get; set; }

        public UpdateAccountCommand(AccountModel account)
        {
            Account = account;
        }
    }
}
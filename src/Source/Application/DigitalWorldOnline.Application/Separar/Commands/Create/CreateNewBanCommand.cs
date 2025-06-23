using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Models.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateNewBanCommand : IRequest<AccountBlockDTO>
    {
        public AccountBlockModel Ban { get; set; }

        public CreateNewBanCommand(AccountBlockModel ban)
        {
            Ban = ban;
        }
    }
}

using DigitalWorldOnline.Commons.DTOs.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateOrUpdateSecondaryPasswordCommand : IRequest
    {
        public long AccountId { get; set; }

        public string SecondaryPassword { get; set; }

        public CreateOrUpdateSecondaryPasswordCommand(long accountId, string secondaryPassword)
        {
            AccountId = accountId;
            SecondaryPassword = secondaryPassword;
        }
    }
}

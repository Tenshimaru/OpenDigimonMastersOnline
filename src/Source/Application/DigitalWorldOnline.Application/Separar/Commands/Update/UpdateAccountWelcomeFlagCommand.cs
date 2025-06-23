using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateAccountWelcomeFlagCommand : IRequest
    {
        public long AccountId { get; private set; }
        public bool WelcomeFlag { get; private set; }

        public UpdateAccountWelcomeFlagCommand(long accountId, bool flag = true)
        {
            AccountId = accountId;
            WelcomeFlag = flag;
        }
    }
}
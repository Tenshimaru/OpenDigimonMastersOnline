using DigitalWorldOnline.Commons.Models.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateAccountMembershipCommand : IRequest
    {
        public long AccountId { get; }
        public DateTime? MembershipExpirationDate { get; }

        public UpdateAccountMembershipCommand(AccountModel account)
        {
            AccountId = account.Id;
            MembershipExpirationDate = account.MembershipExpirationDate;
        }

        public UpdateAccountMembershipCommand(long accountId, DateTime? membershipExpirationDate)
        {
            AccountId = accountId;
            MembershipExpirationDate = membershipExpirationDate;
        }
    }
}
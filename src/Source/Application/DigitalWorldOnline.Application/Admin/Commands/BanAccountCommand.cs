using DigitalWorldOnline.Commons.Enums.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class BanAccountCommand : IRequest
    {
        public long AccountId { get; }
        public AccountBlockEnum Type { get; }
        public string Reason { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public BanAccountCommand(
            long accountId,
            AccountBlockEnum type,
            string reason,
            DateTime startDate,
            DateTime endDate)
        {
            AccountId = accountId;
            Type = type;
            Reason = reason;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}

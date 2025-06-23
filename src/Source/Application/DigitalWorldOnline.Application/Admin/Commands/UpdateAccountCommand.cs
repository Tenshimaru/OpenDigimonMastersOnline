using DigitalWorldOnline.Commons.Enums.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateAccountCommand : IRequest
    {
        public long Id { get; }
        public string Username { get; }
        public string Email { get; }
        public string? SecondaryPassword { get; }
        public string? DiscordId { get; }
        public AccountAccessLevelEnum AccessLevel { get; }
        public int Premium { get; }
        public int Silk { get; }
        public DateTime? MembershipExpirationDate { get; }
        public bool ReceiveWelcome { get; }

        public UpdateAccountCommand(
            long id,
            string username,
            string email,
            string? secondaryPassword,
            string? discordId,
            AccountAccessLevelEnum accessLevel,
            int premium,
            int silk,
            DateTime? membershipExpirationDate,
            bool receiveWelcome)
        {
            Id = id;
            Username = username;
            Email = email;
            SecondaryPassword = secondaryPassword;
            DiscordId = discordId;
            AccessLevel = accessLevel;
            Premium = premium;
            Silk = silk;
            MembershipExpirationDate = membershipExpirationDate;
            ReceiveWelcome = receiveWelcome;
        }
    }
}
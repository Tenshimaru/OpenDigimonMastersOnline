using DigitalWorldOnline.Commons.Enums.Account;

namespace DigitalWorldOnline.Commons.ViewModel.Account
{
    public class AccountUpdateViewModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Account username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Account e-mail.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Account access level.
        /// </summary>
        public AccountAccessLevelEnum AccessLevel { get; set; }

        /// <summary>
        /// Account premium coins.
        /// </summary>
        public int Premium { get; set; }

        /// <summary>
        /// Account silk(bônus) coins.
        /// </summary>
        public int Silk { get; set; }

        /// <summary>
        /// Account secondary password.
        /// </summary>
        public string? SecondaryPassword { get; set; }

        /// <summary>
        /// Account Discord ID.
        /// </summary>
        public string? DiscordId { get; set; }

        /// <summary>
        /// Account membership expiration date.
        /// </summary>
        public DateTime? MembershipExpirationDate { get; set; }

        /// <summary>
        /// Flag to receive welcome message.
        /// </summary>
        public bool ReceiveWelcome { get; set; }

        /// <summary>
        /// Account creation date.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Account last connection date.
        /// </summary>
        public DateTime? LastConnection { get; set; }

        /// <summary>
        /// Flag for empty fields.
        /// </summary>
        public bool Empty => string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Email);

        public AccountUpdateViewModel() { }

        public AccountUpdateViewModel(
            long id,
            string username,
            string email,
            string? secondaryPassword,
            string? discordId,
            AccountAccessLevelEnum accessLevel,
            int premium,
            int silk,
            DateTime? membershipExpirationDate,
            bool receiveWelcome,
            DateTime createDate,
            DateTime? lastConnection)
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
            CreateDate = createDate;
            LastConnection = lastConnection;
        }
    }
}
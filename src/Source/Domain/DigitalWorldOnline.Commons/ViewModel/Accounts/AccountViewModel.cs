using DigitalWorldOnline.Commons.Enums.Account;

namespace DigitalWorldOnline.Commons.ViewModel.Accounts
{
    public class AccountViewModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Account username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Account password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Account e-mail.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Account access level.
        /// </summary>
        public AccountAccessLevelEnum AccessLevel { get; set; }

        /// <summary>
        /// Account creation date.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Account last connection date.
        /// </summary>
        public DateTime? LastConnection { get; set; }

        /// <summary>
        /// Account premium coins.
        /// </summary>
        public int Premium { get; set; }

        /// <summary>
        /// Account silk (bonus) coins.
        /// </summary>
        public int Silk { get; set; }
    }
}

using DigitalWorldOnline.Commons.Enums.Account;

namespace DigitalWorldOnline.Commons.ViewModel.Account
{
    public class AccountCreationViewModel
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
        /// Account premium coins.
        /// </summary>
        public int Premium { get; set; }

        /// <summary>
        /// Account silk(bônus) coins.
        /// </summary>
        public int Silk { get; set; }

        /// <summary>
        /// Flag for empty fields.
        /// </summary>
        public bool Empty => string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password);

        public AccountCreationViewModel()
        {
            AccessLevel = AccountAccessLevelEnum.Default;
        }
    }
}
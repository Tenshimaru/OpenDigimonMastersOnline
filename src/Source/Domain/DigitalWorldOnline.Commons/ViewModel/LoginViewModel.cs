using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.ViewModel
{
    public class LoginViewModel
    {
        /// <summary>
        /// Username used to log-in.
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Password used to log-in.
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// User permissions.
        /// </summary>
        public UserAccessLevelEnum AccessLevel { get; set; }

        /// <summary>
        /// Flag for empty fields.
        /// </summary>
        public bool Empty => string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password);

    }
}

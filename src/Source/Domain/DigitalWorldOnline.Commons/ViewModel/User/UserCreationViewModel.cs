using DigitalWorldOnline.Commons.Enums;
using System;

namespace DigitalWorldOnline.Commons.ViewModel.User
{
    public class UserCreationViewModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// User access level.
        /// </summary>
        public UserAccessLevelEnum AccessLevel { get; set; }

        /// <summary>
        /// Flag for empty fields.
        /// </summary>
        public bool Empty => string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password);

        public UserCreationViewModel()
        {
            AccessLevel = UserAccessLevelEnum.Default;
        }
    }
}

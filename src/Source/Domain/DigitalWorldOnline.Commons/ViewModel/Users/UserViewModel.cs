using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.ViewModel.Users
{
    public class UserViewModel
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
        /// User access level.
        /// </summary>
        public UserAccessLevelEnum AccessLevel { get; set; }
    }
}

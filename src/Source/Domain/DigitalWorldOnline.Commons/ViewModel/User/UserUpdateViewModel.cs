using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.ViewModel.User
{
    public class UserUpdateViewModel
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

        /// <summary>
        /// Flag for empty fields.
        /// </summary>
        public bool Empty => string.IsNullOrEmpty(Username);

        public UserUpdateViewModel() { }

        public UserUpdateViewModel(
            long id,
            string username,
            UserAccessLevelEnum accessLevel)
        {
            Id = id;
            Username = username;
            AccessLevel = accessLevel;
        }
    }
}
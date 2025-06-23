namespace DigitalWorldOnline.Api.Dtos.In
{
    public class CreateAccountIn
    {
        /// <summary>
        /// Endereço de e-mail.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Discord Identifier.
        /// </summary>
        public string DiscordId { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Password.
        /// </summary>
        public string Password { get; set; }
    }
}

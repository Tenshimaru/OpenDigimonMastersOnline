using DigitalWorldOnline.Commons.Enums.Account;
using System.ComponentModel.DataAnnotations;

namespace DigitalWorldOnline.Commons.DTOs.Account
{
    public class LoginTryDTO
    {
        public long Id { get; set; }

        [MinLength(6)]
        public string Username { get; set; }

        public DateTime Date { get; set; }

        public string Ip { get; set; }

        public LoginTryResultEnum Result { get; set; }
    }
}

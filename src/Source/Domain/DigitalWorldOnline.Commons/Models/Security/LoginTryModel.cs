using DigitalWorldOnline.Commons.Enums.Account;

namespace DigitalWorldOnline.Commons.Models.Security
{
    public class LoginTryModel
    {
        public string Username { get; private set; }

        public DateTime Date { get; private set; }

        public string Ip { get; private set; }

        public LoginTryResultEnum Result { get; set; }

        public LoginTryModel(string username, DateTime date, string ip, LoginTryResultEnum result)
        {
            Username = username;
            Date = date;
            Ip = ip;
            Result = result;
        }
    }
}

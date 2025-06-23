using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Models.Character;

namespace DigitalWorldOnline.Commons.Models.Account
{
    public class AccountBlockModel
    {
        public long Id { get; set; }

        public long AccountId { get; set; }

        public AccountBlockEnum Type { get; set; }

        public string Reason { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public static AccountBlockModel Create(int AccountID, string Reason, AccountBlockEnum type, DateTime End)
        {
            var ban = new AccountBlockModel()
            {
                AccountId = AccountID,
                Type = type,
                Reason = Reason,
                StartDate = DateTime.Now,
                EndDate = End,
            };

            return ban;
        }
    }
}

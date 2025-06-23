using DigitalWorldOnline.Commons.Enums.Account;

namespace DigitalWorldOnline.Commons.DTOs.Account
{
    public class AccountBlockDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        public AccountBlockEnum Type { get; set; }

        public string Reason { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public long AccountId { get; set; }
        public AccountDTO Account { get; set; }
    }
}

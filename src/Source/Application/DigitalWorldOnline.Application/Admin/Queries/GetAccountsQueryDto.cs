using DigitalWorldOnline.Commons.DTOs.Account;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetAccountsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<AccountDTO> Registers { get; set; }
    }
}
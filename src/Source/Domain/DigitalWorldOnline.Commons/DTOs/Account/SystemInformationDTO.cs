namespace DigitalWorldOnline.Commons.DTOs.Account
{
    public class SystemInformationDTO
    {
        public long Id { get; set; }

        public string? Cpu { get; set; }
        
        public string? Gpu { get; set; }
        
        public string? Ip { get; set; }

        public long AccountId { get; set; }
        public AccountDTO Account { get; set; }
    }
}

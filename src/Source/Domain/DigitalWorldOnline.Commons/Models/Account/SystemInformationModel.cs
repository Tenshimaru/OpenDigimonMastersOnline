namespace DigitalWorldOnline.Commons.Models.Account
{
    public class SystemInformationModel
    {
        public long Id { get; set; }

        public long AccountId { get; set; }

        public string? Cpu { get; set; }

        public string? Gpu { get; set; }

        public string? Ip { get; set; }

        public SystemInformationModel(long id, long accountId, string? cpu, string? gpu, string? ip)
        {
            Id = id;
            AccountId = accountId;
            Cpu = cpu;
            Gpu = gpu;
            Ip = ip;
        }

        public SystemInformationModel(long accountId, string? cpu, string? gpu, string? ip)
        {
            AccountId = accountId;
            Cpu = cpu;
            Gpu = gpu;
            Ip = ip;
        }
    }
}

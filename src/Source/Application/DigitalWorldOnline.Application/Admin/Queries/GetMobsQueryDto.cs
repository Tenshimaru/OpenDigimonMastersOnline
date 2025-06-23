using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetMobsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<MobConfigDTO> Registers { get; set; }
    }
}
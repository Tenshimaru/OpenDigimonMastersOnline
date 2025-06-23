using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSummonMobsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<SummonMobDTO> Registers { get; set; }
    }
}
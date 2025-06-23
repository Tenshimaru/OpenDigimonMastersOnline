using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSummonsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<SummonDTO> Registers { get; set; }
    }
}
using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetHatchConfigsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<HatchConfigDTO> Registers { get; set; }
    }
}
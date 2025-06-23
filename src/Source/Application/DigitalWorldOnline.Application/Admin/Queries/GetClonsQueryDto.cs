using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetClonsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<CloneConfigDTO> Registers { get; set; }
    }
}
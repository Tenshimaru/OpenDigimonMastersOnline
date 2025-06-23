using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetMapsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<MapConfigDTO> Registers { get; set; }
    }
}
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventMapMobsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<EventMobConfigDTO> Registers { get; set; }
    }
}
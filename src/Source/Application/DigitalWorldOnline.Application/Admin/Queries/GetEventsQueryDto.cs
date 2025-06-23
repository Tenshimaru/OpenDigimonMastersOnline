using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<EventConfigDTO> Registers { get; set; }
    }
}
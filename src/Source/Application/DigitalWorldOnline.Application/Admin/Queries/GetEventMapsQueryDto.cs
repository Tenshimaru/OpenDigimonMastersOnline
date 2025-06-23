using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventMapsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<EventMapsConfigDTO> Registers { get; set; }
    }
}
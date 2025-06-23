using DigitalWorldOnline.Commons.DTOs.Server;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetServersQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<ServerDTO> Registers { get; set; }
    }
}
using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetUsersQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<UserDTO> Registers { get; set; }
    }
}
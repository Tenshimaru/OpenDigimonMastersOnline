using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetContainersQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<ContainerAssetDTO> Registers { get; set; }
    }
}
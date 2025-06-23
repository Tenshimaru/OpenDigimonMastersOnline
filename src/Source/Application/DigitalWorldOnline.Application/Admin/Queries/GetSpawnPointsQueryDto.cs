using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSpawnPointsQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<MapRegionAssetDTO> Registers { get; set; }
    }
}
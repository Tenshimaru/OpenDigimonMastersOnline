using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSpawnPointByIdQueryDto
    {
        public long MapId { get; set; }
        public string MapName { get; set; }
        public MapRegionAssetDTO? Register { get; set; }
    }
}
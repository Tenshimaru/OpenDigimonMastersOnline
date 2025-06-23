using DigitalWorldOnline.Commons.DTOs.Assets;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateSpawnPointCommand : IRequest<MapRegionAssetDTO>
    {
        public int MapId { get; }
        public MapRegionAssetDTO SpawnPoint { get; }

        public CreateSpawnPointCommand(MapRegionAssetDTO spawnPoint, int mapId)
        {
            SpawnPoint = spawnPoint;
            MapId = mapId;
        }
    }
}
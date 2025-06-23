using MediatR;
using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MapRegionListAssetsByMapIdQuery : IRequest<MapRegionListAssetDTO?>
    {
        public int MapId { get; private set; }

        public MapRegionListAssetsByMapIdQuery(int mapId)
        {
            MapId = mapId;
        }
    }
}
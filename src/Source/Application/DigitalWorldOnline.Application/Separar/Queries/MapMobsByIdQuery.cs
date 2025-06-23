using MediatR;
using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MapMobsByIdQuery : IRequest<List<MobConfigDTO>>
    {
        public int MapId { get; private set; }

        public MapMobsByIdQuery(int mapId)
        {
            MapId = mapId;
        }
    }
}
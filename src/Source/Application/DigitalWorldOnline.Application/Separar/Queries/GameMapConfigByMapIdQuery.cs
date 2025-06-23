using MediatR;
using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GameMapConfigByMapIdQuery : IRequest<MapConfigDTO?>
    {
        public int MapId { get; private set; }

        public GameMapConfigByMapIdQuery(int mapId)
        {
            MapId = mapId;
        }
    }
}
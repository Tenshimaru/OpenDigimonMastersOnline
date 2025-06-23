using MediatR;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GameMapsConfigQuery : IRequest<List<MapConfigDTO>>
    {
        public MapTypeEnum Type { get; }

        public GameMapsConfigQuery(MapTypeEnum type = MapTypeEnum.Default)
        {
            Type = type;
        }
    }
}
using MediatR;
using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GameMapConfigByIdQuery : IRequest<MapConfigDTO?>
    {
        public long Id { get; private set; }

        public GameMapConfigByIdQuery(long id)
        {
            Id = id;
        }
    }
}
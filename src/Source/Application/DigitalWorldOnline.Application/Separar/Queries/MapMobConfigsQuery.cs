using MediatR;
using DigitalWorldOnline.Commons.DTOs.Config;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MapMobConfigsQuery : IRequest<IList<MobConfigDTO>>
    {
        public long MapConfigId { get; private set; }

        public MapMobConfigsQuery(long mapConfigId)
        {
            MapConfigId = mapConfigId;
        }

    }
}
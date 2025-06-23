using MediatR;
using DigitalWorldOnline.Commons.DTOs.Shop;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ConsignedShopsQuery : IRequest<IList<ConsignedShopDTO>>
    {
        public int MapId { get; private set; }

        public ConsignedShopsQuery(int mapId)
        {
            MapId = mapId;
        }
    }
}
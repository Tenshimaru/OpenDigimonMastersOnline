using MediatR;
using DigitalWorldOnline.Commons.DTOs.Shop;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ConsignedShopByTamerIdQuery : IRequest<ConsignedShopDTO?>
    {
        public long CharacterId { get; private set; }

        public ConsignedShopByTamerIdQuery(long characterId)
        {
            CharacterId = characterId;
        }
    }
}
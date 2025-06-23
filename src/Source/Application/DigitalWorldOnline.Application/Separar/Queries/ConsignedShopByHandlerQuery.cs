using MediatR;
using DigitalWorldOnline.Commons.DTOs.Shop;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ConsignedShopByHandlerQuery : IRequest<ConsignedShopDTO>
    {
        public long GeneralHandler { get; private set; }

        public ConsignedShopByHandlerQuery(long generalHandler)
        {
            GeneralHandler = generalHandler;
        }
    }
}
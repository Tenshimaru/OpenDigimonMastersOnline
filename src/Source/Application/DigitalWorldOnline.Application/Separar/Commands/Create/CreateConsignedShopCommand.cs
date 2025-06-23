using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.DTOs.Shop;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateConsignedShopCommand : IRequest<ConsignedShopDTO>
    {
        public ConsignedShop ConsignedShop { get; set; }

        public CreateConsignedShopCommand(ConsignedShop consignedShop)
        {
            ConsignedShop = consignedShop;
        }
    }
}
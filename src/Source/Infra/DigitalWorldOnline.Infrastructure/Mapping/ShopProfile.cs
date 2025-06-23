using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Models.TamerShop;


namespace DigitalWorldOnline.Infrastructure.Mapping
{
    public class ShopProfile : Profile
    {
        public ShopProfile()
        {

            CreateMap<ConsignedShop, ConsignedShopDTO>()
                .ReverseMap();

            CreateMap<ConsignedShopLocation, ConsignedShopLocationDTO>()
               .ReverseMap();
        }
    }
}

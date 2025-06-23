using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Models.TamerShop;

namespace DigitalWorldOnline.Infrastructure.Mapping
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<ConsignedShop, ConsignedShopDTO>()
                .ReverseMap();

            CreateMap<GameMap, MapConfigDTO>()
                .ReverseMap();

            CreateMap<GuildModel, GuildDTO>()
                .ReverseMap();

            CreateMap<GuildMemberModel, GuildMemberDTO>()
                .ReverseMap();

            CreateMap<GuildAuthorityModel, GuildAuthorityDTO>()
                .ReverseMap();

            CreateMap<GuildSkillModel, GuildSkillDTO>()
                .ReverseMap();

            CreateMap<GuildHistoricModel, GuildHistoricDTO>()
                .ReverseMap();

            CreateMap<ItemListModel, ItemListDTO>()
                .ReverseMap();

            CreateMap<ItemModel, ItemDTO>()
                .ReverseMap();

            CreateMap<ItemAccessoryStatusModel, ItemAccessoryStatusDTO>()
                .ReverseMap();

            CreateMap<ItemSocketStatusModel, ItemSocketStatusDTO>()
                .ReverseMap();
        }
    }
}
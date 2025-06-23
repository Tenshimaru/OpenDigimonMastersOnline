using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.ViewModel.Accounts;
using DigitalWorldOnline.Commons.ViewModel.Clones;
using DigitalWorldOnline.Commons.ViewModel.Containers;
using DigitalWorldOnline.Commons.ViewModel.Events;
using DigitalWorldOnline.Commons.ViewModel.Hatchs;
using DigitalWorldOnline.Commons.ViewModel.Maps;
using DigitalWorldOnline.Commons.ViewModel.Mobs;
using DigitalWorldOnline.Commons.ViewModel.Players;
using DigitalWorldOnline.Commons.ViewModel.Scans;
using DigitalWorldOnline.Commons.ViewModel.Servers;
using DigitalWorldOnline.Commons.ViewModel.SpawnPoint;
using DigitalWorldOnline.Commons.ViewModel.Users;

namespace DigitalWorldOnline.Infrastructure.Mapping
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<UserDTO, UserViewModel>();

            CreateMap<ServerDTO, ServerViewModel>();

            CreateMap<AccountDTO, AccountViewModel>();

            CreateMap<MapConfigDTO, MapViewModel>()
                .ForMember(dest => dest.MobsCount, x => x.MapFrom(src => src.Mobs.Count));

            CreateMap<MobConfigDTO, MobViewModel>()
                .ReverseMap();

            CreateMap<MobLocationConfigDTO, MobLocationViewModel>()
                .ReverseMap();

            CreateMap<MobExpRewardConfigDTO, MobExpRewardViewModel>()
                .ReverseMap();

            CreateMap<MobDropRewardConfigDTO, MobDropRewardViewModel>()
                .ReverseMap();

            CreateMap<ItemDropConfigDTO, MobItemDropViewModel>()
                .ReverseMap();

            CreateMap<BitsDropConfigDTO, MobBitDropViewModel>()
                .ReverseMap();

            CreateMap<EventMapsConfigDTO, EventMapViewModel>()
                .ForMember(dest => dest.MobsCount, x => x.MapFrom(src => src.Mobs.Count));

            CreateMap<EventMobConfigDTO, MobViewModel>()
                .ReverseMap();

            CreateMap<EventMobLocationConfigDTO, MobLocationViewModel>()
                .ReverseMap();

            CreateMap<EventMobExpRewardConfigDTO, MobExpRewardViewModel>()
                .ReverseMap();

            CreateMap<EventMobDropRewardConfigDTO, MobDropRewardViewModel>()
                .ReverseMap();

            CreateMap<EventItemDropConfigDTO, MobItemDropViewModel>()
                .ReverseMap();

            CreateMap<EventBitsDropConfigDTO, MobBitDropViewModel>()
                .ReverseMap();

            CreateMap<MapRegionAssetDTO, SpawnPointViewModel>()
                .ReverseMap();

            CreateMap<MapRegionAssetDTO, SpawnPointCreationViewModel>()
                .ReverseMap();

            CreateMap<MapRegionAssetDTO, SpawnPointUpdateViewModel>()
                .ReverseMap();

            CreateMap<ScanDetailAssetDTO, ScanDetailViewModel>()
                .ReverseMap();

            CreateMap<ScanRewardDetailAssetDTO, ScanRewardDetailViewModel>()
                .ReverseMap();

            CreateMap<ContainerAssetDTO, ContainerViewModel>()
                .ReverseMap();

            CreateMap<ContainerRewardAssetDTO, ContainerRewardViewModel>()
                .ReverseMap();

            CreateMap<CloneConfigDTO, CloneViewModel>()
                .ReverseMap();

            CreateMap<HatchConfigDTO, HatchViewModel>()
                .ReverseMap();

            CreateMap<CharacterDTO, PlayerViewModel>()
                .ForMember(x => x.MapId, src => src.MapFrom(y => y.Location.MapId))
                .ReverseMap();
        }
    }
}
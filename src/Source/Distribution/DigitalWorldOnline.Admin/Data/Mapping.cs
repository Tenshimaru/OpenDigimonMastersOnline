using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Models.Config.Events;
using DigitalWorldOnline.Commons.ViewModel.Asset;
using DigitalWorldOnline.Commons.ViewModel.Events;
using DigitalWorldOnline.Commons.ViewModel.Mobs;
using DigitalWorldOnline.Commons.ViewModel.Summons;
using DigitalWorldOnline.Application.Admin.Queries;

public class AdminMappingProfile :Profile
{
    public AdminMappingProfile()
    {
        CreateMap<MonsterBaseInfoAssetDTO,MobAssetViewModel>();
        CreateMap<MobAssetViewModel,MobCreationViewModel>();
        CreateMap<MobAssetViewModel,MobUpdateViewModel>();
        CreateMap<MobCreationViewModel,MobConfigDTO>();
        CreateMap<MobLocationViewModel,MobLocationConfigDTO>();
        CreateMap<MobExpRewardViewModel,MobExpRewardConfigDTO>();
        CreateMap<MobDropRewardViewModel,MobDropRewardConfigDTO>();
        CreateMap<MobItemDropViewModel,ItemDropConfigDTO>();
        CreateMap<MobBitDropViewModel,BitsDropConfigDTO>();
        CreateMap<MobBitDropViewModel,BitsDropConfigDTO>();
        CreateMap<SummonMobAssetViewModel,SummonMobUpdateViewModel>();

        CreateMap<SummonViewModel,SummonDTO>()
            .ReverseMap();
        CreateMap<SummonMobDropRewardDTO,SummonMobDropRewardViewModel>().ReverseMap();
        CreateMap<SummonMobExpRewardDTO,SummonMobExpRewardViewModel>().ReverseMap();
        CreateMap<SummonMobLocationDTO,SummonMobLocationViewModel>().ReverseMap();
        CreateMap<SummonMobDTO,SummonMobUpdateViewModel>().ReverseMap();
        CreateMap<SummonMobAssetViewModel,SummonMobViewModel>();

        CreateMap<SummonMobDTO,SummonMobViewModel>().ReverseMap();
        CreateMap<SummonMobDTO,SummonMobAssetViewModel>().ReverseMap();

        CreateMap<MobAssetViewModel,SummonMobViewModel>();
        CreateMap<ItemAssetDTO,ItemAssetViewModel>();
        CreateMap<MobConfigDTO,MobUpdateViewModel>()
            .ReverseMap();

        CreateMap<EventViewModel,EventConfigDTO>()
            .ReverseMap();

        CreateMap<EventMapViewModel,EventMapsConfigDTO>()
            .ReverseMap();

        CreateMap<EventBitsDropConfigModel,EventBitsDropConfigDTO>()
            .ReverseMap();

        CreateMap<EventItemDropConfigModel,EventItemDropConfigDTO>()
            .ReverseMap();

        CreateMap<EventMobConfigModel,EventMobConfigDTO>()
            .ReverseMap();

        CreateMap<MonsterBaseInfoAssetDTO,EventMobAssetViewModel>();
        CreateMap<EventMobAssetViewModel,EventMobCreationViewModel>();
        CreateMap<EventMobAssetViewModel,EventMobUpdateViewModel>();
        CreateMap<EventMobConfigDTO,EventMobUpdateViewModel>();
        CreateMap<EventMobCreationViewModel,EventMobConfigDTO>();

        CreateMap<EventMobAssetViewModel,EventMobConfigModel>()
            .ForMember(dest => dest.ExpReward,opt => opt.Ignore())
            .ForMember(dest => dest.Location,opt => opt.Ignore())
            .ForMember(dest => dest.DropReward,opt => opt.Ignore())
            .ForMember(dest => dest.Duration,opt => opt.Ignore());

        CreateMap<EventMobDropRewardConfigModel,EventMobDropRewardConfigDTO>()
            .ReverseMap();

        CreateMap<EventMobExpRewardConfigModel,EventMobExpRewardConfigDTO>()
            .ReverseMap();

        CreateMap<EventMobLocationConfigModel,EventMobLocationConfigDTO>()
            .ReverseMap();

        CreateMap<MapConfigDTO,MapConfigViewModel>();

     
        CreateMap<SummonMobBitDropDTO,SummonMobBitDropViewModel>().ReverseMap();

        CreateMap<SummonMobDropRewardDTO,SummonMobDropRewardViewModel>()
            .ForMember(dest => dest.BitsDrop,opt => opt.MapFrom(src => src.BitsDrop))
            .ReverseMap();

        CreateMap<SummonMobItemDropDTO,SummonMobItemDropViewModel>()
    .ReverseMap();

        CreateMap<SummonMobDropRewardDTO,SummonMobDropRewardViewModel>()
            .ForMember(dest => dest.Drops,opt => opt.MapFrom(src => src.Drops))
            .ReverseMap();

        CreateMap<SummonMobDTO,SummonMobUpdateViewModel>()
            .ForMember(dest => dest.DropReward,opt => opt.MapFrom(src => src.DropReward))
            .ReverseMap();
    }
}


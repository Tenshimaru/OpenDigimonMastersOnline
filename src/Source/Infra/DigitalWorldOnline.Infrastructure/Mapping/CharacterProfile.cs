using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.Model.Character;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Events;
using DigitalWorldOnline.Commons.Models.Mechanics;

namespace DigitalWorldOnline.Infrastructure.Mapping
{
    public class CharacterProfile : Profile
    {
        public CharacterProfile()
        {
            CreateMap<CharacterModel, CharacterDTO>()
                .ForMember(x => x.Guild, y => y.Ignore())
                .ReverseMap();

            CreateMap<CharacterLocationModel, CharacterLocationDTO>()
                .ReverseMap();

            CreateMap<CharacterIncubatorModel, CharacterIncubatorDTO>()
                .ReverseMap();

            CreateMap<CharacterMapRegionModel, CharacterMapRegionDTO>()
                .ReverseMap();

            CreateMap<CharacterBuffListModel, CharacterBuffListDTO>()
                .ReverseMap();

            CreateMap<CharacterBuffModel, CharacterBuffDTO>()
                .ReverseMap();

            CreateMap<CharacterSealListModel, CharacterSealListDTO>()
                .ReverseMap();

            CreateMap<CharacterSealModel, CharacterSealDTO>()
                .ReverseMap();

            CreateMap<CharacterXaiModel, CharacterXaiDTO>()
                .ReverseMap();

            CreateMap<XaiAssetDTO, CharacterXaiModel>()
                .ForMember(dest => dest.Id, x => x.Ignore());

            CreateMap<TimeRewardDTO, TimeRewardModel>()
                .ForMember(dest => dest.Id, x => x.Ignore())
                .ReverseMap();

            CreateMap<AttendanceRewardDTO, AttendanceRewardModel>()
                .ForMember(dest => dest.Id, x => x.Ignore())
                .ReverseMap();

            CreateMap<CharacterFriendModel, CharacterFriendDTO>()
                .ReverseMap();

            CreateMap<CharacterFoeModel, CharacterFoeDTO>()
                .ReverseMap();

            CreateMap<CharacterProgressModel, CharacterProgressDTO>()
                .ReverseMap();

            CreateMap<InProgressQuestModel, InProgressQuestDTO>()
                .ReverseMap();

            CreateMap<CharacterActiveEvolutionModel, CharacterActiveEvolutionDTO>()
                .ReverseMap();

            CreateMap<CharacterDigimonArchiveModel, CharacterDigimonArchiveDTO>()
                .ReverseMap();

            CreateMap<CharacterDigimonArchiveItemModel, CharacterDigimonArchiveItemDTO>()
                .ReverseMap();

            CreateMap<CharacterArenaPointsModel, CharacterArenaPointsDTO>()
                .ReverseMap();

            CreateMap<CharacterArenaDailyPointsModel, CharacterArenaDailyPointsDTO>()
                .ForMember(dest => dest.Id, x => x.Ignore())
                .ReverseMap();

            CreateMap<CharacterTamerSkillModel, CharacterTamerSkillDTO>()
                .ForMember(dest => dest.Id, x => x.Ignore())
                .ReverseMap();

            CreateMap<CharacterEncyclopediaModel, CharacterEncyclopediaDTO>()
                .ReverseMap();

            CreateMap<CharacterEncyclopediaEvolutionsModel, CharacterEncyclopediaEvolutionsDTO>()
                .ReverseMap();

            CreateMap<CharacterEncyclopediaEvolutionsDTO, CharacterEncyclopediaEvolutionsModel>()
                .ReverseMap();
        }
    }
}
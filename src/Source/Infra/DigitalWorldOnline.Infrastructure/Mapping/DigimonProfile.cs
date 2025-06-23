using AutoMapper;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.DTOs.Digimon;

namespace DigitalWorldOnline.Infrastructure.Mapping
{
    public class DigimonProfile : Profile
    {
        public DigimonProfile()
        {
            CreateMap<DigimonModel, DigimonDTO>()
                .ForMember(dest => dest.Character, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<DigimonLocationModel, DigimonLocationDTO>()
                .ReverseMap();

            CreateMap<DigimonDigicloneModel, DigimonDigicloneDTO>()
                .ReverseMap();
            
            CreateMap<DigimonDigicloneHistoryModel, DigimonDigicloneHistoryDTO>()
                .ReverseMap();

            CreateMap<DigimonAttributeExperienceModel, DigimonAttributeExperienceDTO>()
                .ReverseMap();

            CreateMap<DigimonEvolutionSkillModel, DigimonEvolutionSkillDTO>()
                .ReverseMap();

            CreateMap<DigimonBuffListModel, DigimonBuffListDTO>()
                .ReverseMap();

            CreateMap<DigimonBuffModel, DigimonBuffDTO>()
                .ReverseMap();

            CreateMap<DigimonEvolutionModel, DigimonEvolutionDTO>()
                .ReverseMap();
        }
    }
}
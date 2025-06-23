using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Routine;
using DigitalWorldOnline.Models.DTOs.Routine;

namespace DigitalWorldOnline.Infrastructure.Mapping
{
    public class RoutineProfile : Profile
    {
        public RoutineProfile()
        {
            CreateMap<RoutineDTO, RoutineModel>();
        }
    }
}
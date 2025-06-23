using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Models.Account;

namespace DigitalWorldOnline.Infrastructure.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountModel, AccountDTO>()
                .ReverseMap();

            CreateMap<AccountBlockModel, AccountBlockDTO>()
                .ReverseMap();

            CreateMap<SystemInformationModel, SystemInformationDTO>()
                .ReverseMap();
        }
    }
}
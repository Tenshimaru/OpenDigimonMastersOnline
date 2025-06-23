using AutoMapper;
using DigitalWorldOnline.Commons.Models.Chat;
using DigitalWorldOnline.Commons.Models.Security;
using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.DTOs.Chat;

namespace DigitalWorldOnline.Infrastructure.Mapping
{
    public class SecurityProfile : Profile
    {
        public SecurityProfile()
        {
            CreateMap<LoginTryModel, LoginTryDTO>()
                .ReverseMap();

            CreateMap<ChatMessageModel, ChatMessageDTO>()
                .ReverseMap();
        }
    }
}
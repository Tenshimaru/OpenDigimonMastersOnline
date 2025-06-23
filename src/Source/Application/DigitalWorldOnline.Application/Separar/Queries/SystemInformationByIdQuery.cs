using MediatR;
using DigitalWorldOnline.Commons.DTOs.Account;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class SystemInformationByIdQuery : IRequest<SystemInformationDTO?>
    {
        public long Id { get; set; }

        public SystemInformationByIdQuery(long id)
        {
            Id = id;
        }
    }
}


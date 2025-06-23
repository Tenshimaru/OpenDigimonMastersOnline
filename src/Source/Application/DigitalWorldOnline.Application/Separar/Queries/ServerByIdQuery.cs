using MediatR;
using DigitalWorldOnline.Commons.DTOs.Server;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ServerByIdQuery : IRequest<ServerDTO?>
    {
        public long Id { get; set; }

        public ServerByIdQuery(long id)
        {
            Id = id;
        }
    }
}


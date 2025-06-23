using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ServersQueryHandler : IRequestHandler<ServersQuery, IList<ServerDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public ServersQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<ServerDTO>> Handle(ServersQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetServersAsync(request.AccessLevel);
        }
    }
}
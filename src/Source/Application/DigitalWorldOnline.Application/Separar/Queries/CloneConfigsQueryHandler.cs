using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CloneConfigsQueryHandler : IRequestHandler<CloneConfigsQuery, List<CloneConfigDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public CloneConfigsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CloneConfigDTO>> Handle(CloneConfigsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetCloneConfigsAsync();
        }
    }
}
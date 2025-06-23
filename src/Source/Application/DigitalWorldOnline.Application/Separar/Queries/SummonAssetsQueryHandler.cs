using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class SummonAssetsQueryHandler : IRequestHandler<SummonAssetsQuery, List<SummonDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public SummonAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SummonDTO>> Handle(SummonAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSummonAssetsAsync();
        }
    }
}

using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class SummonMobAssetsQueryHandler : IRequestHandler<SummonMobAssetsQuery, List<SummonMobDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public SummonMobAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SummonMobDTO>> Handle(SummonMobAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSummonMobAssetsAsync();
        }
    }
}

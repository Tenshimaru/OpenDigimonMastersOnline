using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MonthlyEventAssetsQueryHandler : IRequestHandler<MonthlyEventAssetsQuery, List<MonthlyEventAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public MonthlyEventAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MonthlyEventAssetDTO>> Handle(MonthlyEventAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMonthlyEventAssetsAsync();
        }
    }
}
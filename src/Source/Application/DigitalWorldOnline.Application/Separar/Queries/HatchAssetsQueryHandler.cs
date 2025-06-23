using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class HatchAssetsQueryHandler : IRequestHandler<HatchAssetsQuery, List<HatchAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public HatchAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<HatchAssetDTO>> Handle(HatchAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetHatchAssetsAsync();
        }
    }
}
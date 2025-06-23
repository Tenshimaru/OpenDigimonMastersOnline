using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class SealStatusAssetsQueryHandler : IRequestHandler<SealStatusAssetsQuery, List<SealDetailAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public SealStatusAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SealDetailAssetDTO>> Handle(SealStatusAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSealStatusAssetsAsync();
        }
    }
}
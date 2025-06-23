using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ScanDetailAssetQueryHandler : IRequestHandler<ScanDetailAssetQuery, List<ScanDetailAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public ScanDetailAssetQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ScanDetailAssetDTO>> Handle(ScanDetailAssetQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetScanDetailAssetsAsync();
        }
    }
}
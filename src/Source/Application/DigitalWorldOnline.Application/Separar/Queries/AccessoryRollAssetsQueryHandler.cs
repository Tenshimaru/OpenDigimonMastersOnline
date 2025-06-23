using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AccessoryRollAssetsQueryHandler : IRequestHandler<AccessoryRollAssetsQuery, List<AccessoryRollAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public AccessoryRollAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AccessoryRollAssetDTO>> Handle(AccessoryRollAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAccessoryRollInfoAsync();
        }
    }
}

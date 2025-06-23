using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class PortalAssetsQueryHandler : IRequestHandler<PortalAssetsQuery, List<PortalAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public PortalAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PortalAssetDTO>> Handle(PortalAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetPortalAssetsAsync();
        }
    }
}

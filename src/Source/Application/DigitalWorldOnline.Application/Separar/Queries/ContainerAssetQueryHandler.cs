using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ContainerAssetQueryHandler : IRequestHandler<ContainerAssetQuery, List<ContainerAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public ContainerAssetQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ContainerAssetDTO>> Handle(ContainerAssetQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetContainerAssetsAsync();
        }
    }
}
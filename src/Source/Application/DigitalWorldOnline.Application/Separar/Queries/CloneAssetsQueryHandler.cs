using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CloneAssetsQueryHandler : IRequestHandler<CloneAssetsQuery, List<CloneAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public CloneAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CloneAssetDTO>> Handle(CloneAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetCloneAssetsAsync();
        }
    }
}
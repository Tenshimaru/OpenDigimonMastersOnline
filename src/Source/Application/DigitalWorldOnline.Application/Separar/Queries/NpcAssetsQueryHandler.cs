using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class NpcAssetsQueryHandler : IRequestHandler<NpcAssetsQuery, List<NpcAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public NpcAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<NpcAssetDTO>> Handle(NpcAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetNpcAssetsAsync();
        }
    }
}
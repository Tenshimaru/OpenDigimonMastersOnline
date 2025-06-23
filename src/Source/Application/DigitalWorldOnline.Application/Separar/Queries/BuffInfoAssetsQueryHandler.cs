using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class BuffInfoAssetsQueryHandler : IRequestHandler<BuffInfoAssetsQuery, List<BuffAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public BuffInfoAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<BuffAssetDTO>> Handle(BuffInfoAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetBuffInfoAssetsAsync();
        }
    }
}
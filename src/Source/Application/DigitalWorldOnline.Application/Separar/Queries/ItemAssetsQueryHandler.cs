using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ItemAssetsQueryHandler : IRequestHandler<ItemAssetsQuery, List<ItemAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public ItemAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ItemAssetDTO>> Handle(ItemAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetItemAssetsAsync();
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ItemCraftAssetsByFilterQueryHandler : IRequestHandler<ItemCraftAssetsByFilterQuery, ItemCraftAssetDTO?>
    {
        private readonly IServerQueriesRepository _repository;

        public ItemCraftAssetsByFilterQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<ItemCraftAssetDTO?> Handle(ItemCraftAssetsByFilterQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetItemCraftAssetsByFilterAsync(request.NpcId, request.SeqId);
        }
    }
}

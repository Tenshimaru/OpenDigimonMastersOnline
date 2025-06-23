using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CashShopAssetsQueryHandler : IRequestHandler<CashShopAssetsQuery, List<CashShopAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public CashShopAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CashShopAssetDTO>> Handle(CashShopAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetCashShopAssetsAsync();
        }
    }
}
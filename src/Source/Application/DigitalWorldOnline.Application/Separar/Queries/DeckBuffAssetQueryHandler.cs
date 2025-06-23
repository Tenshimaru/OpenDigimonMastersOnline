using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DeckBuffAssetsQueryHandler : IRequestHandler<DeckBuffAssetsQuery, List<DeckBuffAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public DeckBuffAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DeckBuffAssetDTO>> Handle(DeckBuffAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDeckBuffAssetsAsync();
        }
    }
}
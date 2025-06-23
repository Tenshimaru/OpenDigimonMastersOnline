using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class EvolutionArmorAssetsQueryHandler : IRequestHandler<EvolutionArmorAssetsQuery, List<EvolutionArmorAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public EvolutionArmorAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EvolutionArmorAssetDTO>> Handle(EvolutionArmorAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEvolutionArmorAssetsAsync();
        }
    }
}
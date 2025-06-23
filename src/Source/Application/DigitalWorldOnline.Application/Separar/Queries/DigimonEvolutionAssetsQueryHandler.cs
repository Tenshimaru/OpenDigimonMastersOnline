using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DigimonEvolutionAssetsQueryHandler : IRequestHandler<DigimonEvolutionAssetsQuery, List<EvolutionAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public DigimonEvolutionAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EvolutionAssetDTO>> Handle(DigimonEvolutionAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDigimonEvolutionAssetsAsync();
        }
    }
}
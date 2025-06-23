using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DigimonEvolutionAssetsByTypeQueryHandler : IRequestHandler<DigimonEvolutionAssetsByTypeQuery, EvolutionAssetDTO?>
    {
        private readonly IServerQueriesRepository _repository;

        public DigimonEvolutionAssetsByTypeQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<EvolutionAssetDTO?> Handle(DigimonEvolutionAssetsByTypeQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDigimonEvolutionAssetsByTypeAsync(request.Type);
        }
    }
}
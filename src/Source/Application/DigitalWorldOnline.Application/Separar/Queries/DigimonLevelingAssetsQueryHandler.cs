using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DigimonLevelingAssetsQueryHandler : IRequestHandler<DigimonLevelingAssetsQuery, List<DigimonLevelStatusAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public DigimonLevelingAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DigimonLevelStatusAssetDTO>> Handle(DigimonLevelingAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDigimonLevelAssetsAsync();
        }
    }
}
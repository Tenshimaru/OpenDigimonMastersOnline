using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DigimonSkillAssetsQueryHandler : IRequestHandler<DigimonSkillAssetsQuery, List<DigimonSkillAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public DigimonSkillAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DigimonSkillAssetDTO>> Handle(DigimonSkillAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDigimonSkillAssetsAsync();
        }
    }
}
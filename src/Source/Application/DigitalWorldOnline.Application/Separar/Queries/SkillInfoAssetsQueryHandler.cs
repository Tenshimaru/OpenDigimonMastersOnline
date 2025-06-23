using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class SkillInfoAssetsQueryHandler : IRequestHandler<SkillInfoAssetsQuery, List<SkillInfoAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public SkillInfoAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SkillInfoAssetDTO>> Handle(SkillInfoAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSkillInfoAssetsAsync();
        }
    }
}
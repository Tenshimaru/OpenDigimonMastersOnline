using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MonsterSkillInfoAssetsQueryHandler : IRequestHandler<MonsterSkillInfoAssetsQuery, List<MonsterSkillInfoAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public MonsterSkillInfoAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MonsterSkillInfoAssetDTO>> Handle(MonsterSkillInfoAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMonsterSkillInfoAssetsAsync();
        }
    }
}
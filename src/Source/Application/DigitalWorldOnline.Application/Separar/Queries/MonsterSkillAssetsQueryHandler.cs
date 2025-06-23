using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MonsterSkillAssetsQueryHandler : IRequestHandler<MonsterSkillAssetsQuery, List<MonsterSkillAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public MonsterSkillAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MonsterSkillAssetDTO>> Handle(MonsterSkillAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMonsterSkillSkillAssetsAsync();
        }
    }
}
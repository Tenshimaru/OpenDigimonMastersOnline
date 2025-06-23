using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class SkillCodeAssetsQueryHandler : IRequestHandler<SkillCodeAssetsQuery, List<SkillCodeAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public SkillCodeAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SkillCodeAssetDTO>> Handle(SkillCodeAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSkillCodeAssetsAsync();
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class QuestAssetsQueryHandler : IRequestHandler<QuestAssetsQuery, List<QuestAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public QuestAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<QuestAssetDTO>> Handle(QuestAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetQuestAssetsAsync();
        }
    }
}

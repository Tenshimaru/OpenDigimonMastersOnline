using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TamerLevelStatusQueryHandler : IRequestHandler<TamerLevelStatusQuery, CharacterLevelStatusAssetDTO>
    {
        private readonly IServerQueriesRepository _repository;

        public TamerLevelStatusQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<CharacterLevelStatusAssetDTO> Handle(TamerLevelStatusQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetTamerLevelingStatusAsync(request.Type, request.Level);
        }
    }
}
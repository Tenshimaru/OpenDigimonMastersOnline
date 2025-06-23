using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TamerBaseStatusQueryHandler : IRequestHandler<TamerBaseStatusQuery, CharacterBaseStatusAssetDTO>
    {
        private readonly IServerQueriesRepository _repository;

        public TamerBaseStatusQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<CharacterBaseStatusAssetDTO> Handle(TamerBaseStatusQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetTamerBaseStatusAsync(request.Type);
        }
    }
}
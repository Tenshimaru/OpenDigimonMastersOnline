using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class StatusApplyAssetQueryHandler : IRequestHandler<StatusApplyAssetQuery, List<StatusApplyAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public StatusApplyAssetQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StatusApplyAssetDTO>> Handle(StatusApplyAssetQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetStatusApplyInfoAsync();
        }
    }
}

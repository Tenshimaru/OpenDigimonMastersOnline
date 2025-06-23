using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GotchaAssetsQueryHandler : IRequestHandler<GotchaAssetsQuery, List<GotchaAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public GotchaAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GotchaAssetDTO>> Handle(GotchaAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetGotchaAssetsAsync();
        }
    }
}

using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TitleStatusAssetsQueryHandler : IRequestHandler<TitleStatusAssetsQuery, TitleStatusAssetDTO?>
    {
        private readonly IServerQueriesRepository _repository;

        public TitleStatusAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<TitleStatusAssetDTO?> Handle(TitleStatusAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetTitleStatusAssetsAsync(request.TitleId);
        }
    }
}
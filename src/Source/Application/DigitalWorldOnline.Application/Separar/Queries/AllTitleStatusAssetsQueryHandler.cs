using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AllTitleStatusAssetsQueryHandler : IRequestHandler<AllTitleStatusAssetsQuery, List<TitleStatusAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public AllTitleStatusAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TitleStatusAssetDTO>> Handle(AllTitleStatusAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllTitleStatusInfoAsync();
        }
    }
}

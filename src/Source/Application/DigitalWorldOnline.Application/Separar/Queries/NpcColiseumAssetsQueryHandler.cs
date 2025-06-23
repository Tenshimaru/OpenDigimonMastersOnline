using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class NpcColiseumAssetsQueryHandler : IRequestHandler<NpcColiseumAssetsQuery, List<NpcColiseumAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public NpcColiseumAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<NpcColiseumAssetDTO>> Handle(NpcColiseumAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetNpcColiseumAssetsAsync();
        }
    }
}
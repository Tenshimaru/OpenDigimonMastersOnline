using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AllDigimonBaseInfoQueryHandler : IRequestHandler<AllDigimonBaseInfoQuery, IList<DigimonBaseInfoAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public AllDigimonBaseInfoQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<DigimonBaseInfoAssetDTO>> Handle(AllDigimonBaseInfoQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllDigimonBaseInfoAsync();
        }
    }
}

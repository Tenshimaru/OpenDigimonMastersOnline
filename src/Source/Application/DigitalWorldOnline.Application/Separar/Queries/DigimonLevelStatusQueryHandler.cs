using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DigimonLevelStatusQueryHandler : IRequestHandler<DigimonLevelStatusQuery, DigimonLevelStatusAssetDTO>
    {
        private readonly IServerQueriesRepository _repository;

        public DigimonLevelStatusQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<DigimonLevelStatusAssetDTO> Handle(DigimonLevelStatusQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDigimonLevelingStatusAsync(request.Type, request.Level);
        }
    }
}
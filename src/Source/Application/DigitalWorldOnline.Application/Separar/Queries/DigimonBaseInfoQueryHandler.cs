using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DigimonBaseInfoQueryHandler : IRequestHandler<DigimonBaseInfoQuery, DigimonBaseInfoAssetDTO?>
    {
        private readonly IServerQueriesRepository _repository;

        public DigimonBaseInfoQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<DigimonBaseInfoAssetDTO?> Handle(DigimonBaseInfoQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDigimonBaseInfoAsync(request.Type);
        }
    }
}

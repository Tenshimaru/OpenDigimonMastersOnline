using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class WelcomeMessagesAssetsQueryHandler : IRequestHandler<WelcomeMessagesAssetsQuery, List<WelcomeMessageConfigDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public WelcomeMessagesAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<WelcomeMessageConfigDTO>> Handle(WelcomeMessagesAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetWelcomeMessagesAssetsAsync();
        }
    }
}
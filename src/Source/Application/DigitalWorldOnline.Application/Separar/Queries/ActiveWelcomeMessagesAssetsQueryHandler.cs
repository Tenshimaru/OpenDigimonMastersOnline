using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ActiveWelcomeMessagesAssetsQueryHandler : IRequestHandler<ActiveWelcomeMessagesAssetsQuery, List<WelcomeMessageConfigDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public ActiveWelcomeMessagesAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<WelcomeMessageConfigDTO>> Handle(ActiveWelcomeMessagesAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetActiveWelcomeMessagesAssetsAsync();
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class EventsConfigQueryHandler : IRequestHandler<EventsConfigQuery, List<EventConfigDTO>>
    {
        private readonly IConfigQueriesRepository _repository;

        public EventsConfigQueryHandler(IConfigQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EventConfigDTO>> Handle(EventsConfigQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventsConfigAsync(request.IsEnabled);
        }
    }
}
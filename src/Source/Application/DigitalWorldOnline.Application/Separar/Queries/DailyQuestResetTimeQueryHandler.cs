using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class DailyQuestResetTimeQueryHandler : IRequestHandler<DailyQuestResetTimeQuery, DateTime>
    {
        private readonly IServerQueriesRepository _repository;

        public DailyQuestResetTimeQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<DateTime> Handle(DailyQuestResetTimeQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDailyQuestResetTimeAsync();
        }
    }
}
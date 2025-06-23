using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TimeRewardAssetsQueryHandler : IRequestHandler<TimeRewardAssetsQuery, List<TimeRewardAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public TimeRewardAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TimeRewardAssetDTO>> Handle(TimeRewardAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetTimeRewardAssetsAsync();
        }
    }

    public class TimeRewardEventsQueryHandler : IRequestHandler<TimeRewardEventsQuery, List<TimeRewardDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public TimeRewardEventsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TimeRewardDTO>> Handle(TimeRewardEventsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetTimeRewardEventsAsync();
        }
    }
}
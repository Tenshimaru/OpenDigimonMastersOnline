using MediatR;
using DigitalWorldOnline.Commons.Interfaces;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ChannelsByMapIdQueryHandler : IRequestHandler<ChannelsByMapIdQuery, IDictionary<byte, byte>>
    {
        private readonly ICharacterQueriesRepository _repository;

        public ChannelsByMapIdQueryHandler(ICharacterQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IDictionary<byte, byte>> Handle(ChannelsByMapIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetChannelsByMapIdAsync(request.MapId);
        }
    }
}

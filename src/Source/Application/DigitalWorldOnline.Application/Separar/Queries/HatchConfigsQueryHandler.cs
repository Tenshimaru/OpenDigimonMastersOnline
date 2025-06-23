using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class HatchConfigsQueryHandler : IRequestHandler<HatchConfigsQuery, List<HatchConfigDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public HatchConfigsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<HatchConfigDTO>> Handle(HatchConfigsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetHatchConfigsAsync();
        }
    }
}
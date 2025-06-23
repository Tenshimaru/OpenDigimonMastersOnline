using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class FruitConfigsQueryHandler : IRequestHandler<FruitConfigsQuery, List<FruitConfigDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public FruitConfigsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FruitConfigDTO>> Handle(FruitConfigsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetFruitConfigsAsync();
        }
    }
}
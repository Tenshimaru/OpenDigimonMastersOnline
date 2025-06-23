using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ConsignedShopsQueryHandler : IRequestHandler<ConsignedShopsQuery, IList<ConsignedShopDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public ConsignedShopsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<ConsignedShopDTO>> Handle(ConsignedShopsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetConsignedShopsAsync(request.MapId);
        }
    }
}

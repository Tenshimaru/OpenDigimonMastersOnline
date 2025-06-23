using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ConsignedShopByHandlerQueryHandler : IRequestHandler<ConsignedShopByHandlerQuery, ConsignedShopDTO>
    {
        private readonly IServerQueriesRepository _repository;

        public ConsignedShopByHandlerQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<ConsignedShopDTO> Handle(ConsignedShopByHandlerQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetConsignedShopByHandlerAsync(request.GeneralHandler);
        }
    }
}

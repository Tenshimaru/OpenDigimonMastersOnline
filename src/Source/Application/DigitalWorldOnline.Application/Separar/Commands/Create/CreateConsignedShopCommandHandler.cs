using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateConsignedShopCommandHandler : IRequestHandler<CreateConsignedShopCommand, ConsignedShopDTO>
    {
        private readonly IServerCommandsRepository _repository;

        public CreateConsignedShopCommandHandler(IServerCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<ConsignedShopDTO> Handle(CreateConsignedShopCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddConsignedShopAsync(request.ConsignedShop);
        }
    }
}

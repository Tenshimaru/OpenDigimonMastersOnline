using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteConsignedShopCommandHandler : IRequestHandler<DeleteConsignedShopCommand>
    {
        private readonly IServerCommandsRepository _repository;

        public DeleteConsignedShopCommandHandler(IServerCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteConsignedShopCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteConsignedShopByHandlerAsync(request.GeneralHandler);

            return Unit.Value;
        }
    }
}

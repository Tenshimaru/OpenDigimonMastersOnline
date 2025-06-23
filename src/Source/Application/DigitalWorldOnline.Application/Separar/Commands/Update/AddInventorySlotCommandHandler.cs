using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class AddInventorySlotCommandHandler : IRequestHandler<AddInventorySlotCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public AddInventorySlotCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(AddInventorySlotCommand request, CancellationToken cancellationToken)
        {
            await _repository.AddInventorySlotAsync(request.NewSlot);

            return Unit.Value;
        }
    }
}
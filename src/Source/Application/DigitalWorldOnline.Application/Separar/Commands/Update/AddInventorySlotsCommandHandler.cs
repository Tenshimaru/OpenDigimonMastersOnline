using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class AddInventorySlotsCommandHandler : IRequestHandler<AddInventorySlotsCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public AddInventorySlotsCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(AddInventorySlotsCommand request, CancellationToken cancellationToken)
        {
            await _repository.AddInventorySlotsAsync(request.Items);

            return Unit.Value;
        }
    }
}
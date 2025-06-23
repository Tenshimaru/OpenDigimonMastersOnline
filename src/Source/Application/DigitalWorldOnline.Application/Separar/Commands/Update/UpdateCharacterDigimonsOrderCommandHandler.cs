using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterDigimonsOrderCommandHandler : IRequestHandler<UpdateCharacterDigimonsOrderCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterDigimonsOrderCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterDigimonsOrderCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterDigimonsOrderAsync(request.Character);

            return Unit.Value;
        }
    }
}
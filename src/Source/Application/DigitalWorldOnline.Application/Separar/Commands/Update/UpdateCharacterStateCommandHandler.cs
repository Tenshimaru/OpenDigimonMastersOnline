using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterStateCommandHandler : IRequestHandler<UpdateCharacterStateCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterStateCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterStateCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterStateByIdAsync(request.CharacterId, request.State);

            return Unit.Value;
        }
    }
}
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterEventStateCommandHandler : IRequestHandler<UpdateCharacterEventStateCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterEventStateCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterEventStateCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterEventStateByIdAsync(request.CharacterId, request.State);

            return Unit.Value;
        }
    }
}
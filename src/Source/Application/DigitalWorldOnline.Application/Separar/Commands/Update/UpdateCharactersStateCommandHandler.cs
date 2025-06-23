using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharactersStateCommandHandler : IRequestHandler<UpdateCharactersStateCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharactersStateCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharactersStateCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharactersStateAsync(request.State);

            return Unit.Value;
        }
    }
}
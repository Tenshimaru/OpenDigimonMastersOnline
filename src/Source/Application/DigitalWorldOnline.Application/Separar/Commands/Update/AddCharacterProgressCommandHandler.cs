using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class AddCharacterProgressCommandHandler : IRequestHandler<AddCharacterProgressCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public AddCharacterProgressCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(AddCharacterProgressCommand request, CancellationToken cancellationToken)
        {
            await _repository.AddCharacterProgressAsync(request.Progress);

            return Unit.Value;
        }
    }
}
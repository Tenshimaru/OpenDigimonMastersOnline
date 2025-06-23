using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigicloneCommandHandler : IRequestHandler<UpdateDigicloneCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateDigicloneCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDigicloneCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateDigicloneAsync(request.Digiclone);

            return Unit.Value;
        }
    }
}

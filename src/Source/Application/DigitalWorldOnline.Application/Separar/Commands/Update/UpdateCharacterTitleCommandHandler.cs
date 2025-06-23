using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterTitleCommandHandler : IRequestHandler<UpdateCharacterTitleCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterTitleCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterTitleCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterTitleByIdAsync(request.CharacterId, request.TitleId);

            return Unit.Value;
        }
    }
}
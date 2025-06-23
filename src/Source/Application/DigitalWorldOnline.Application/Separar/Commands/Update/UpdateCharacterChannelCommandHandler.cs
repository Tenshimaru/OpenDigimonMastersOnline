using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterChannelCommandHandler : IRequestHandler<UpdateCharacterChannelCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterChannelCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterChannelCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterChannelByIdAsync(request.CharacterId, request.Channel);

            return Unit.Value;
        }
    }
}
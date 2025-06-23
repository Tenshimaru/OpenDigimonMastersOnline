using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterInitialPacketSentOnceSentCommandHandler : IRequestHandler<UpdateCharacterInitialPacketSentOnceSentCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterInitialPacketSentOnceSentCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterInitialPacketSentOnceSentCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterInitialPacketSentOnceSentAsync(request.CharacterId, request.InitialPacketSentOnceSent);

            return Unit.Value;
        }
    }
}

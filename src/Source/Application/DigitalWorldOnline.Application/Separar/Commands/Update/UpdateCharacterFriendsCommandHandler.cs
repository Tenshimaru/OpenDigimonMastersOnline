using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterFriendsCommandHandler : IRequestHandler<UpdateCharacterFriendsCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterFriendsCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterFriendsCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterFriendsAsync(request.Character, request.Connected);

            return Unit.Value;
        }
    }
}
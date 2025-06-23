using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateNewFriendCommandHandler : IRequestHandler<CreateNewFriendCommand, CharacterFriendDTO>
    {
        private readonly ICharacterCommandsRepository _repository;

        public CreateNewFriendCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CharacterFriendDTO> Handle(CreateNewFriendCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddFriendAsync(request.Friend);
        }
    }
}
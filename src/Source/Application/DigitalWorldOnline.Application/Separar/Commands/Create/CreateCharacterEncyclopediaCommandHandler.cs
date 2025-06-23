using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateCharacterEncyclopediaCommandHandler : IRequestHandler<CreateCharacterEncyclopediaCommand, CharacterEncyclopediaModel>
    {
        private readonly ICharacterCommandsRepository _repository;

        public CreateCharacterEncyclopediaCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CharacterEncyclopediaModel> Handle(CreateCharacterEncyclopediaCommand request, CancellationToken cancellationToken)
        {
            return await _repository.CreateCharacterEncyclopediaAsync(request.CharacterEncyclopedia);
        }
    }
}
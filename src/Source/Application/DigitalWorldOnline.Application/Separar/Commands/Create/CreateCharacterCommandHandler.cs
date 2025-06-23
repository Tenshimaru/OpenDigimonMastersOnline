using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateCharacterCommandHandler : IRequestHandler<CreateCharacterCommand, long>
    {
        private readonly ICharacterCommandsRepository _repository;

        public CreateCharacterCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<long> Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddCharacterAsync(request.Character);
        }
    }
}
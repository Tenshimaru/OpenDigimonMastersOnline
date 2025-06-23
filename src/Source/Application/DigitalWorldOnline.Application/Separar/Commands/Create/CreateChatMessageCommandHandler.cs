using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateChatMessageCommandHandler : IRequestHandler<CreateChatMessageCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public CreateChatMessageCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateChatMessageCommand request, CancellationToken cancellationToken)
        {
            await _repository.AddChatMessageAsync(request.ChatMessage);

            return Unit.Value;
        }
    }
}
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigimonSizeCommandHandler : IRequestHandler<UpdateDigimonSizeCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateDigimonSizeCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDigimonSizeCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateDigimonSizeAsync(request.DigimonId, request.Size);

            return Unit.Value;
        }
    }
}

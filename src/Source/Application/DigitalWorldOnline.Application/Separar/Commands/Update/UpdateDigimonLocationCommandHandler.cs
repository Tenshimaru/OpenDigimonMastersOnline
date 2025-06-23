using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigimonLocationCommandHandler : IRequestHandler<UpdateDigimonLocationCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateDigimonLocationCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDigimonLocationCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateDigimonLocationAsync(request.Location);

            return Unit.Value;
        }
    }
}
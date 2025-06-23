using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigimonBuffListCommandHandler : IRequestHandler<UpdateDigimonBuffListCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateDigimonBuffListCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDigimonBuffListCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateDigimonBuffListAsync(request.BuffList);

            return Unit.Value;
        }
    }
}
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigimonGradeCommandHandler : IRequestHandler<UpdateDigimonGradeCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateDigimonGradeCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDigimonGradeCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateDigimonGradeAsync(request.DigimonId, request.Grade);

            return Unit.Value;
        }
    }
}

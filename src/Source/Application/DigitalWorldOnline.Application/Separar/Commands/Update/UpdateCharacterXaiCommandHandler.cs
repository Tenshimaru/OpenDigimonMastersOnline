using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterXaiCommandHandler : IRequestHandler<UpdateCharacterXaiCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterXaiCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterXaiCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterXaiAsync(request.Xai);

            return Unit.Value;
        }
    }
}
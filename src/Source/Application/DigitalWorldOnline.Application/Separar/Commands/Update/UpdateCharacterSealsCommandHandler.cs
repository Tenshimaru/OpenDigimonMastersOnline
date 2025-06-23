using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterSealsCommandHandler : IRequestHandler<UpdateCharacterSealsCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterSealsCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterSealsCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterSealsAsync(request.SealList);

            return Unit.Value;
        }
    }
}
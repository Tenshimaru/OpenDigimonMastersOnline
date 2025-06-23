using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdatePartnerCurrentTypeCommandHandler : IRequestHandler<UpdatePartnerCurrentTypeCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdatePartnerCurrentTypeCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdatePartnerCurrentTypeCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdatePartnerCurrentTypeAsync(request.Digimon);

            return Unit.Value;
        }
    }
}

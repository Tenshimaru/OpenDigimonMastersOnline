using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterBuffListCommandHandler : IRequestHandler<UpdateCharacterBuffListCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterBuffListCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterBuffListCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterBuffListAsync(request.BuffList);

            return Unit.Value;
        }
    }
}
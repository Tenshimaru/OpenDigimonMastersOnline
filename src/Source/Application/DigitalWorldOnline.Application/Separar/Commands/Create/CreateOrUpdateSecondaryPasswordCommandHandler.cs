using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateOrUpdateSecondaryPasswordCommandHandler : IRequestHandler<CreateOrUpdateSecondaryPasswordCommand>
    {
        private readonly IAccountCommandsRepository _repository;

        public CreateOrUpdateSecondaryPasswordCommandHandler(IAccountCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateOrUpdateSecondaryPasswordCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateOrUpdateSecondaryPasswordByIdAsync(request.AccountId, request.SecondaryPassword);

            return Unit.Value;
        }
    }
}
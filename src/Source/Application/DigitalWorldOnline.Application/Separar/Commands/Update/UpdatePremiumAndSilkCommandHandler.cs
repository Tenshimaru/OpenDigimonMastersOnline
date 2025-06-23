using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdatePremiumAndSilkCommandHandler : IRequestHandler<UpdatePremiumAndSilkCommand>
    {
        private readonly IAccountCommandsRepository _repository;

        public UpdatePremiumAndSilkCommandHandler(IAccountCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdatePremiumAndSilkCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdatePremiumAndSilkByIdAsync(request.AccountId, request.Premium, request.Silk);

            return Unit.Value;
        }
    }
}

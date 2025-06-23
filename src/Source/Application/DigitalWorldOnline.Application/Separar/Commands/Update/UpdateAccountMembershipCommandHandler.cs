using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateAccountMembershipCommandHandler : IRequestHandler<UpdateAccountMembershipCommand>
    {
        private readonly IAccountCommandsRepository _repository;

        public UpdateAccountMembershipCommandHandler(IAccountCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateAccountMembershipCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateAccountMembershipAsync(request.AccountId, request.MembershipExpirationDate);

            return Unit.Value;
        }
    }
}
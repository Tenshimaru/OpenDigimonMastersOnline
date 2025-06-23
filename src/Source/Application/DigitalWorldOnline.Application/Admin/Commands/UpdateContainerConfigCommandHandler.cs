using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateContainerConfigCommandHandler : IRequestHandler<UpdateContainerConfigCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdateContainerConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateContainerConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateContainerConfigAsync(request.Container);

            return Unit.Value;
        }
    }
}
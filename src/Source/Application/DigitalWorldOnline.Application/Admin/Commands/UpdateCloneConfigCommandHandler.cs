using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateCloneConfigCommandHandler : IRequestHandler<UpdateCloneConfigCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdateCloneConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCloneConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCloneConfigAsync(request.Clone);

            return Unit.Value;
        }
    }
}
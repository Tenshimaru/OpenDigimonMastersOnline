using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteContainerConfigCommandHandler : IRequestHandler<DeleteContainerConfigCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteContainerConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteContainerConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteContainerConfigAsync(request.Id);

            return Unit.Value;
        }
    }
}
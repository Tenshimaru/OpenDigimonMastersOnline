using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteSpawnPointCommandHandler : IRequestHandler<DeleteSpawnPointCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteSpawnPointCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteSpawnPointCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteSpawnPointAsync(request.Id);

            return Unit.Value;
        }
    }
}
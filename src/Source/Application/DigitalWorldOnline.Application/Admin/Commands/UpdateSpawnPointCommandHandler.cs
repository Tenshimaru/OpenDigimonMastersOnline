using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateSpawnPointCommandHandler : IRequestHandler<UpdateSpawnPointCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdateSpawnPointCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateSpawnPointCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateSpawnPointAsync(request.SpawnPoint, request.MapId);

            return Unit.Value;
        }
    }
}
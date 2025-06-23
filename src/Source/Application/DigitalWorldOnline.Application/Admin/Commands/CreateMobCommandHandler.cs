using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateMobCommandHandler : IRequestHandler<CreateMobCommand, MobConfigDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public CreateMobCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<MobConfigDTO> Handle(CreateMobCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddMobAsync(request.Mob);
        }
    }
}
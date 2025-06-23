using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateEventMobCommandHandler : IRequestHandler<CreateEventMobCommand, EventMobConfigDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public CreateEventMobCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<EventMobConfigDTO> Handle(CreateEventMobCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddEventMobAsync(request.Mob);
        }
    }
}
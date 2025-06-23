using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateEventConfigCommandHandler : IRequestHandler<CreateEventConfigCommand, EventConfigDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public CreateEventConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<EventConfigDTO> Handle(CreateEventConfigCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddEventConfigAsync(request.Event);
        }
    }
}
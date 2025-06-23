using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateEventMapConfigCommandHandler : IRequestHandler<CreateEventMapConfigCommand, EventMapsConfigDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public CreateEventMapConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<EventMapsConfigDTO> Handle(CreateEventMapConfigCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddEventMapConfigAsync(request.EventMap);
        }
    }
}
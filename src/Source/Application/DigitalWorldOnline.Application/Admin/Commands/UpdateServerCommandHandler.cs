using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdateServerCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateServerCommand request, CancellationToken cancellationToken)
        {
            var dto = new ServerDTO()
            {
                Id = request.Id,
                Name = request.Name,
                Experience = request.Experience,
                Maintenance = request.Maintenance,
                Type = request.Type,
                Port = request.Port
            };

            await _repository.UpdateServerAsync(dto);

            return Unit.Value;
        }
    }
}
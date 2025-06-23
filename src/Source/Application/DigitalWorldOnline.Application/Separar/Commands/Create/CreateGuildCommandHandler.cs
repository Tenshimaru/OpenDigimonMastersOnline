using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateGuildCommandHandler : IRequestHandler<CreateGuildCommand, GuildDTO>
    {
        private readonly IServerCommandsRepository _repository;

        public CreateGuildCommandHandler(IServerCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<GuildDTO> Handle(CreateGuildCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddGuildAsync(request.Guild);
        }
    }
}
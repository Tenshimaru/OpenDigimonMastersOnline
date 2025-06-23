using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateCloneConfigCommandHandler : IRequestHandler<CreateCloneConfigCommand, CloneConfigDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public CreateCloneConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CloneConfigDTO> Handle(CreateCloneConfigCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddCloneConfigAsync(request.Clone);
        }
    }
}
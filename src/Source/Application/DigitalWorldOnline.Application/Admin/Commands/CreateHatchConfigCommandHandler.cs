using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateHatchConfigCommandHandler : IRequestHandler<CreateHatchConfigCommand, HatchConfigDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public CreateHatchConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<HatchConfigDTO> Handle(CreateHatchConfigCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddHatchConfigAsync(request.Hatch);
        }
    }
}
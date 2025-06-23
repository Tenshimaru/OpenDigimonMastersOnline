using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateScanConfigCommandHandler : IRequestHandler<CreateScanConfigCommand, ScanDetailAssetDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public CreateScanConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<ScanDetailAssetDTO> Handle(CreateScanConfigCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddScanConfigAsync(request.Scan);
        }
    }
}
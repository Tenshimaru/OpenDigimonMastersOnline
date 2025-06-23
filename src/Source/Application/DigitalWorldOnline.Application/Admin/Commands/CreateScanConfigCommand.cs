using DigitalWorldOnline.Commons.DTOs.Assets;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateScanConfigCommand : IRequest<ScanDetailAssetDTO>
    {
        public ScanDetailAssetDTO Scan { get; }

        public CreateScanConfigCommand(ScanDetailAssetDTO scan)
        {
            Scan = scan;
        }
    }
}
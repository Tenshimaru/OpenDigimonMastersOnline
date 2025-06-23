using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetScansQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<ScanDetailAssetDTO> Registers { get; set; }
    }
}
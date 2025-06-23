using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Enums.Character;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetPlayersQueryDto
    {
        public int TotalRegisters { get; set; }
        public List<CharacterDTO> Registers { get; set; }
    }
}
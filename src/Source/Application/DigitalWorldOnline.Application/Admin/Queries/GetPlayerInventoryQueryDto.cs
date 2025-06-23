using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetPlayerInventoryQueryDto
    {
        public CharacterDTO? Player { get; set; }
    }
}

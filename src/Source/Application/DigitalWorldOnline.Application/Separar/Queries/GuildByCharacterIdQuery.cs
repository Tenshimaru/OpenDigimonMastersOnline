using DigitalWorldOnline.Commons.DTOs.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GuildByCharacterIdQuery : IRequest<GuildDTO?>
    {
        public long CharacterId { get; private set; }

        public GuildByCharacterIdQuery(long characterId)
        {
            CharacterId = characterId;
        }
    }
}
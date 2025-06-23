using MediatR;
using DigitalWorldOnline.Commons.DTOs.Digimon;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetCharacterNameAndGuildByIdQuery : IRequest<(string TamerName, string GuildName)>
    {
        public long CharacterId { get; }

        public GetCharacterNameAndGuildByIdQuery(long characterId)
        {
            CharacterId = characterId;
        }
    }
}
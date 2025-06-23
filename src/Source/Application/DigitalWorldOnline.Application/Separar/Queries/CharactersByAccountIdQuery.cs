using MediatR;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharactersByAccountIdQuery : IRequest<IList<CharacterDTO>>
    {
        public long AccountId { get; set; }

        public CharactersByAccountIdQuery(long accountId)
        {
            AccountId = accountId;
        }
    }
}


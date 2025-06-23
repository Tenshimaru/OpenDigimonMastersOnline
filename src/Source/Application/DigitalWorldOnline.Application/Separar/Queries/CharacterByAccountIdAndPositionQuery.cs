using MediatR;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharacterByAccountIdAndPositionQuery : IRequest<CharacterDTO?>
    {
        public long AccountId { get; set; }

        public byte Position { get; set; }

        public CharacterByAccountIdAndPositionQuery(long accountId, byte position)
        {
            AccountId = accountId;
            Position = position;
        }
    }
}


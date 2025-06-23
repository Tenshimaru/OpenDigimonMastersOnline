using DigitalWorldOnline.Commons.DTOs.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterChannelCommand : IRequest
    {
        public long CharacterId { get; set; }

        public byte Channel { get; set; }

        public UpdateCharacterChannelCommand(long characterId, byte? channel = byte.MaxValue)
        {
            CharacterId = characterId;
            Channel = channel ?? 255;
        }
    }
}

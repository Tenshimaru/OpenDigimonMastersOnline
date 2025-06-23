using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterSizeCommand : IRequest
    {
        public long CharacterId { get; }
        public short Size { get; }

        public UpdateCharacterSizeCommand(long characterId, short size)
        {
            CharacterId = characterId;
            Size = size;
        }
    }
}

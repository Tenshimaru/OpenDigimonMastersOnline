using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterDigimonSlotsCommand : IRequest
    {
        public long CharacterId { get; }
        public byte Slots { get; }

        public UpdateCharacterDigimonSlotsCommand(long characterId, byte slots)
        {
            CharacterId = characterId;
            Slots = slots;
        }
    }
}
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigimonSlotCommand : IRequest
    {
        public long DigimonId { get; set; }
        public byte DigimonSlot { get; set; }

        public UpdateDigimonSlotCommand(long digimonId, byte digimonSlot)
        {
            DigimonId = digimonId;
            DigimonSlot = digimonSlot;
        }
    }
}
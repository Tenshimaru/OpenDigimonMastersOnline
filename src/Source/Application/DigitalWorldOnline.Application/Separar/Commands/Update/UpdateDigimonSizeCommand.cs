using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigimonSizeCommand : IRequest
    {
        public long DigimonId { get; }
        public short Size { get; }

        public UpdateDigimonSizeCommand(long digimonId, short size)
        {
            DigimonId = digimonId;
            Size = size;
        }
    }
}

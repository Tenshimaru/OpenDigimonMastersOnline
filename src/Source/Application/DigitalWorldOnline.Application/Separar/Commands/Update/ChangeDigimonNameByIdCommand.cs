using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.DTOs.Digimon;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class ChangeDigimonNameByIdCommand : IRequest<DigimonDTO>
    {
        public long DigimonId { get; set; }
        public string NewDigimonName { get; set; }

        public ChangeDigimonNameByIdCommand(long digimonId, string newDigimonName)
        {
            DigimonId = digimonId;
            NewDigimonName = newDigimonName;
        }
    }
}

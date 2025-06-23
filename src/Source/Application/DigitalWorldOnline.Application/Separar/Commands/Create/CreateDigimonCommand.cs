using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.Models.Digimon;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateDigimonCommand : IRequest<DigimonDTO>
    {
        public DigimonModel Digimon { get; set; }

        public CreateDigimonCommand(DigimonModel digimon)
        {
            Digimon = digimon;
        }
    }
}

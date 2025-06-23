using DigitalWorldOnline.Commons.Models.Digimon;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateEvolutionCommand : IRequest
    {
        public DigimonEvolutionModel Evolution { get; }

        public UpdateEvolutionCommand(DigimonEvolutionModel evolution)
        {
            Evolution = evolution;
        }
    }
}
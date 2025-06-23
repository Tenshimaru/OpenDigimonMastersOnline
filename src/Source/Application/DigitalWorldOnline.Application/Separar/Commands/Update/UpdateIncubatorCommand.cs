using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateIncubatorCommand : IRequest
    {
        public CharacterIncubatorModel Incubator { get; }

        public UpdateIncubatorCommand(CharacterIncubatorModel incubator)
        {
            Incubator = incubator;
        }
    }
}
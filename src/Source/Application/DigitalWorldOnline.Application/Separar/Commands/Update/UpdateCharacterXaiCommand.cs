using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterXaiCommand : IRequest
    {
        public CharacterXaiModel Xai { get; }

        public UpdateCharacterXaiCommand(CharacterXaiModel xai)
        {
            Xai = xai;
        }
    }
}
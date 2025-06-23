using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterSealsCommand : IRequest
    {
        public CharacterSealListModel SealList { get; set; }

        public UpdateCharacterSealsCommand(CharacterSealListModel sealList)
        {
            SealList = sealList;
        }
    }
}
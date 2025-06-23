using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Enums;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class ChangeTamerModelByIdCommand : IRequest<CharacterDTO>
    {
        public long CharacterId { get; set; } // Você pode usar um identificador para identificar o personagem a ser modificado.
        public CharacterModelEnum Model { get; set; }

        public ChangeTamerModelByIdCommand(long characterId, CharacterModelEnum newCharacterModel)
        {
            CharacterId = characterId;
            Model = newCharacterModel;
        }
    }
}

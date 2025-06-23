using DigitalWorldOnline.Commons.DTOs.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class ChangeTamerIdTPCommand : IRequest<CharacterDTO>
    {
        public long CharacterId { get; set; } // Você pode usar um identificador para identificar o personagem a ser modificado.
        public int NewTargetTamerIdTP { get; set; }

        public ChangeTamerIdTPCommand(long characterId, int newCharacterName)
        {
            CharacterId = characterId;
            NewTargetTamerIdTP = newCharacterName;
        }
    }
}

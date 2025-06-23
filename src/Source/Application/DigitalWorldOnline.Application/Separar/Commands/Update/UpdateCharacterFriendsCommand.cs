using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterFriendsCommand : IRequest
    {
        public CharacterModel? Character { get; set; }
        public bool Connected { get; set; }

        public UpdateCharacterFriendsCommand(CharacterModel? character, bool connected = false)
        {
            Character = character;
            Connected = connected;
        }
    }
}
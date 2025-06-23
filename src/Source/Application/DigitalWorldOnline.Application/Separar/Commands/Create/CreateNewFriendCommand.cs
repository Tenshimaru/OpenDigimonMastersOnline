using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateNewFriendCommand : IRequest<CharacterFriendDTO>
    {
        public CharacterFriendModel Friend { get; set; }

        public CreateNewFriendCommand(CharacterFriendModel friend)
        {
            Friend = friend;
        }
    }
}

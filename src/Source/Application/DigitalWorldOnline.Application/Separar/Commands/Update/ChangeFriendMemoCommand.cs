using DigitalWorldOnline.Commons.DTOs.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class ChangeFriendMemoCommand : IRequest<CharacterFriendDTO>
    {
        public long Id { get; set; }
        public string NewMemo { get; set; }

        public ChangeFriendMemoCommand(long friendId, string newMemo)
        {
            Id = friendId;
            NewMemo = newMemo;
        }
    }
}

using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterInitialPacketSentOnceSentCommand : IRequest
    {
        public long CharacterId { get; }
        public bool InitialPacketSentOnceSent { get; }

        public UpdateCharacterInitialPacketSentOnceSentCommand(long characterId, bool sendOnceSent = false)
        {
            CharacterId = characterId;
            InitialPacketSentOnceSent = sendOnceSent;
        }
    }
}

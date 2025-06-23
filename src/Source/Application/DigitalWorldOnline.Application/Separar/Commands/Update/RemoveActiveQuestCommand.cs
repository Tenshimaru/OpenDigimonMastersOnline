using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class RemoveActiveQuestCommand : IRequest
    {
        public Guid? ProgressQuestId { get; set; }

        public RemoveActiveQuestCommand(Guid? progressQuestId)
        {
            ProgressQuestId = progressQuestId;
        }
    }
}
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Routines.Commands
{
    public class ExecuteDailyQuestsRoutineCommandHandler : IRequestHandler<ExecuteDailyQuestsRoutineCommand>
    {
        private readonly IRoutineRepository _repository;

        public ExecuteDailyQuestsRoutineCommandHandler(IRoutineRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(ExecuteDailyQuestsRoutineCommand request, CancellationToken cancellationToken)
        {
            await _repository.ExecuteDailyQuestsAsync(request.QuestIdList);

            return Unit.Value;
        }
    }
}
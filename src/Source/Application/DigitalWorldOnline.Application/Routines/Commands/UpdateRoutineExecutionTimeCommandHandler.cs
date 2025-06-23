using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Routines.Commands
{
    public class UpdateRoutineExecutionTimeCommandHandler : IRequestHandler<UpdateRoutineExecutionTimeCommand>
    {
        private readonly IRoutineRepository _repository;

        public UpdateRoutineExecutionTimeCommandHandler(IRoutineRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateRoutineExecutionTimeCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateRoutineExecutionTimeAsync(request.RoutineId);

            return Unit.Value;
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{

    public class UpdateTamerAttendanceTimeRewardCommandHandler : IRequestHandler<UpdateTamerAttendanceTimeRewardCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateTamerAttendanceTimeRewardCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateTamerAttendanceTimeRewardCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateTamerTimeRewardAsync(request.TimeRewardModel);

            return Unit.Value;
        }
    }
}
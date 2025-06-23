using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{

    public class UpdateTamerAttendanceRewardCommandHandler : IRequestHandler<UpdateTamerAttendanceRewardCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateTamerAttendanceRewardCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }


        public async Task<Unit> Handle(UpdateTamerAttendanceRewardCommand request, CancellationToken cancellationToken)
        {
             await _repository.UpdateTamerAttendanceRewardAsync(request.AttendanceRewardModel);

            return Unit.Value;
        }
    }
}
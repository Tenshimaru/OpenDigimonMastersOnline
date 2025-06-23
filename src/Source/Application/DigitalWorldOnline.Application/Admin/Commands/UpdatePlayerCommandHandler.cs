using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, bool>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdatePlayerCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            return await _repository.UpdatePlayerAsync(
                request.Id,
                request.Name,
                request.Level,
                request.CurrentExperience,
                request.MapId,
                request.State,
                request.EventState,
                request.Channel,
                request.Model,
                request.Size,
                request.CurrentHp,
                request.CurrentDs,
                request.XGauge,
                request.XCrystals,
                request.CurrentTitle,
                request.DigimonSlots);
        }
    }
}

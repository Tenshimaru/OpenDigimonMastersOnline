using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{

    public class UpdateTamerTimeRewardCommandHandler : IRequestHandler<UpdateTamerTimeRewardCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateTamerTimeRewardCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateTamerTimeRewardCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateTamerTimeRewardAsync(request.TimeRewardModel);

            return Unit.Value;
        }
    }
}
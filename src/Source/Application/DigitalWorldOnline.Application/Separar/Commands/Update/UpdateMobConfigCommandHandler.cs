using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateMobConfigCommandHandler : IRequestHandler<UpdateMobConfigCommand>
    {
        private readonly IConfigCommandsRepository _repository;

        public UpdateMobConfigCommandHandler(IConfigCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateMobConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateMobConfigAsync(request.MobConfig);

            return Unit.Value;
        }
    }
}
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateMapConfigCommandHandler : IRequestHandler<UpdateMapConfigCommand>
    {
        private readonly IConfigCommandsRepository _repository;

        public UpdateMapConfigCommandHandler(IConfigCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateMapConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateMapConfigAsync(request.MapConfig);

            return Unit.Value;
        }
    }
}
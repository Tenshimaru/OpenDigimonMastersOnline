using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteMobConfigCommandHandler : IRequestHandler<DeleteMobConfigCommand>
    {
        private readonly IConfigCommandsRepository _repository;

        public DeleteMobConfigCommandHandler(IConfigCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteMobConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteMobConfigAsync(request.Id);

            return Unit.Value;
        }
    }
}
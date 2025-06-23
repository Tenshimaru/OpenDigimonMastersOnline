using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteMapConfigCommandHandler : IRequestHandler<DeleteMapConfigCommand>
    {
        private readonly IConfigCommandsRepository _repository;

        public DeleteMapConfigCommandHandler(IConfigCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteMapConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteMapConfigAsync(request.Id);

            return Unit.Value;
        }
    }
}
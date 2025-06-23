using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteAdminUserCommandHandler : IRequestHandler<DeleteAdminUserCommand>
    {
        private readonly IConfigCommandsRepository _repository;

        public DeleteAdminUserCommandHandler(IConfigCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteAdminUserCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAdminUserAsync(request.UserId);

            return Unit.Value;
        }
    }
}
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteEventConfigCommandHandler : IRequestHandler<DeleteEventConfigCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteEventConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteEventConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteEventConfigAsync(request.Id);

            return Unit.Value;
        }
    }
}
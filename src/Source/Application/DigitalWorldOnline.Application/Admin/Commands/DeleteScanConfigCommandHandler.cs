using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteScanConfigCommandHandler : IRequestHandler<DeleteScanConfigCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteScanConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteScanConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteScanConfigAsync(request.Id);

            return Unit.Value;
        }
    }
}
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteMobCommandHandler : IRequestHandler<DeleteMobCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteMobCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteMobCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteMobAsync(request.Id);

            return Unit.Value;
        }
    }
}
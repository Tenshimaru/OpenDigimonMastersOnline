using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteMapMobsCommandHandler : IRequestHandler<DeleteMapMobsCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteMapMobsCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteMapMobsCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteMapMobsAsync(request.Id);

            return Unit.Value;
        }
    }
}
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteEventMapMobsCommandHandler : IRequestHandler<DeleteEventMapMobsCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteEventMapMobsCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteEventMapMobsCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteEventMapMobsAsync(request.Id);

            return Unit.Value;
        }
    }
}
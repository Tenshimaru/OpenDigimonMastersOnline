using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteEventMobCommandHandler : IRequestHandler<DeleteEventMobCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteEventMobCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteEventMobCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteEventMobAsync(request.Id);

            return Unit.Value;
        }
    }
}
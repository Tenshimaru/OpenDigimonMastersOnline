using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteSummonMobCommandHandler : IRequestHandler<DeleteSummonMobCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteSummonMobCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteSummonMobCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteSummonMobAsync(request.Id);

            return Unit.Value;
        }
    }
}
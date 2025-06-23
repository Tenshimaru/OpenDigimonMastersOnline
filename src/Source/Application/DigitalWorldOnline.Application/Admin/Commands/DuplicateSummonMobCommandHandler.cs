using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DuplicateSummonMobCommandHandler : IRequestHandler<DuplicateSummonMobCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DuplicateSummonMobCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DuplicateSummonMobCommand request, CancellationToken cancellationToken)
        {
            await _repository.DuplicateSummonMobAsync(request.Id);

            return Unit.Value;
        }
    }
}
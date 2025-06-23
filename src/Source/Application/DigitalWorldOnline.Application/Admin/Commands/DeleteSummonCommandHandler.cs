using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteSummonCommandHandler : IRequestHandler<DeleteSummonCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public DeleteSummonCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteSummonCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteSummonAsync(request.Id);

            return Unit.Value;
        }
    }
}
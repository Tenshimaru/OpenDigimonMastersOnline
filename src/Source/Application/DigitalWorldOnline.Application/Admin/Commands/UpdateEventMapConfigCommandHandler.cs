using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateEventMapConfigCommandHandler : IRequestHandler<UpdateEventMapConfigCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdateEventMapConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateEventMapConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateEventMapConfigAsync(request.EventMap);

            return Unit.Value;
        }
    }
}
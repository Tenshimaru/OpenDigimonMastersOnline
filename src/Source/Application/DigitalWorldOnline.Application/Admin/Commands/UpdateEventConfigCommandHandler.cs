using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateEventConfigCommandHandler : IRequestHandler<UpdateEventConfigCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdateEventConfigCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateEventConfigCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateEventConfigAsync(request.Event);

            return Unit.Value;
        }
    }
}
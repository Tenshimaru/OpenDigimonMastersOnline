using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdateUserCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var dto = new UserDTO()
            {
                Id = request.Id,
                Username = request.UserName,
                AccessLevel = request.AccessLevel
            };

            await _repository.UpdateUserAsync(dto);

            return Unit.Value;
        }
    }
}
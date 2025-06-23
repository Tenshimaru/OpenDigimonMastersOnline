using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Security;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateLoginTryCommandHandler : IRequestHandler<CreateLoginTryCommand, LoginTryDTO>
    {
        private readonly IAccountCommandsRepository _repository;

        public CreateLoginTryCommandHandler(IAccountCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<LoginTryDTO> Handle(CreateLoginTryCommand request, CancellationToken cancellationToken)
        {
            var loginTry = new LoginTryModel(request.Username, DateTime.Now, request.IpAddress, request.Result);

            return await _repository.AddLoginTryAsync(loginTry);
        }
    }
}
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateSystemInformationCommandHandler : IRequestHandler<UpdateSystemInformationCommand>
    {
        private readonly IAccountCommandsRepository _repository;

        public UpdateSystemInformationCommandHandler(IAccountCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateSystemInformationCommand request, CancellationToken cancellationToken)
        {
            var systemInformation = new SystemInformationModel(request.Id, request.AccountId, request.Cpu, request.Gpu, request.Ip);

            await _repository.UpdateSystemInformationAsync(systemInformation);

            return Unit.Value;
        }
    }
}
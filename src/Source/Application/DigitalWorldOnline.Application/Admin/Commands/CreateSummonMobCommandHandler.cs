using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateSummonMobCommandHandler : IRequestHandler<CreateSummonMobCommand,SummonMobDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public CreateSummonMobCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<SummonMobDTO> Handle(CreateSummonMobCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddSummonMobAsync(request.Mob);
        }
    }
}
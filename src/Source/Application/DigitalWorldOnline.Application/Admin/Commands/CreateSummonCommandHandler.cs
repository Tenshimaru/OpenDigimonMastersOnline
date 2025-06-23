using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateSummonCommandHandler :IRequestHandler<CreateSummonCommand,SummonDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdateSummonCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<SummonDTO> Handle(CreateSummonCommand request,CancellationToken cancellationToken)
        {
            return await _repository.AddSummonConfigAsync(request.Summon);
        }
    }
}

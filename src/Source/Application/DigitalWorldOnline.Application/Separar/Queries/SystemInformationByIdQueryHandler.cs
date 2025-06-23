using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class SystemInformationByIdQueryHandler : IRequestHandler<SystemInformationByIdQuery, SystemInformationDTO?>
    {
        private readonly IAccountQueriesRepository _repository;

        public SystemInformationByIdQueryHandler(IAccountQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<SystemInformationDTO?> Handle(SystemInformationByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSystemInformationByIdAsync(request.Id);
        }
    }
}

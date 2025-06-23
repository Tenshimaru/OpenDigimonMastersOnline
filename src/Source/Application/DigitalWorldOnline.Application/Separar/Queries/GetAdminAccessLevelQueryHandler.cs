using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetAdminAccessLevelQueryHandler : IRequestHandler<GetAdminAccessLevelQuery, UserAccessLevelEnum>
    {
        private readonly IServerQueriesRepository _repository;

        public GetAdminAccessLevelQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserAccessLevelEnum> Handle(GetAdminAccessLevelQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAdminAccessLevelAsync(request.Username);
        }
    }
}

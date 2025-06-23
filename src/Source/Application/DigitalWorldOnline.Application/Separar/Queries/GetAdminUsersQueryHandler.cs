using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, List<UserDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public GetAdminUsersQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UserDTO>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAdminUsersAsync();
        }
    }
}

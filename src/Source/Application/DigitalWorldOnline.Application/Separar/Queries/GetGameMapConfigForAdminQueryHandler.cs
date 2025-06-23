using AutoMapper;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetGameMapConfigForAdminQueryHandler : IRequestHandler<GetGameMapConfigForAdminQuery, List<GetGameMapConfigForAdminQueryDto>>
    {
        private readonly IServerQueriesRepository _repository;
        private readonly IMapper _mapper;

        public GetGameMapConfigForAdminQueryHandler(IServerQueriesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<GetGameMapConfigForAdminQueryDto>> Handle(GetGameMapConfigForAdminQuery request, CancellationToken cancellationToken)
        {
            var maps = await _repository.GetGameMapConfigsForAdminAsync();

            return _mapper.Map<List<GetGameMapConfigForAdminQueryDto>>(maps);
        }
    }
}
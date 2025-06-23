using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSummonMobByIdQuery : IRequest<GetSummonMobByIdQueryDto>
    {
        public long Id { get; }

        public GetSummonMobByIdQuery(long id)
        {
            Id = id;
        }
    }
}
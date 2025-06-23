using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetAccountByIdQuery : IRequest<GetAccountByIdQueryDto>
    {
        public long Id { get; }

        public GetAccountByIdQuery(long id)
        {
            Id = id;
        }
    }
}
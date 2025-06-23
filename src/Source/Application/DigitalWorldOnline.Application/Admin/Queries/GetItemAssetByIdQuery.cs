using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetItemAssetByIdQuery : IRequest<GetItemAssetByIdQueryDto>
    {
        public int Id { get; }

        public GetItemAssetByIdQuery(int id)
        {
            Id = id;
        }
    }
}
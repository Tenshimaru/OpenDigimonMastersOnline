using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetRaidAssetQuery : IRequest<GetRaidAssetQueryDto>
    {
        public string Filter { get; }

        public GetRaidAssetQuery(string filter)
        {
            Filter = filter;
        }
    }
}
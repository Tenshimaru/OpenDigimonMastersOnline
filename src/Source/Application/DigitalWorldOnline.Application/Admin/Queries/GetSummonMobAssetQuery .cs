using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSummonMobAssetQuery : IRequest<GetSummonMobAssetQueryDto>
    {
        public string Filter { get; }

        public GetSummonMobAssetQuery(string filter)
        {
            Filter = filter;
        }
    }
}
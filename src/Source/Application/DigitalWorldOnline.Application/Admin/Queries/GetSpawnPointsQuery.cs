using DigitalWorldOnline.Commons.Enums.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSpawnPointsQuery : IRequest<GetSpawnPointsQueryDto>
    {
        public int MapId { get; }
        public int Limit { get; }
        public int Offset { get; }
        public string SortColumn { get; }
        public SortDirectionEnum SortDirection { get; }

        public GetSpawnPointsQuery(
            int mapId,
            int page,
            int pageSize,
            string sortColumn,
            SortDirectionEnum sortDirection)
        {
            MapId = mapId;
            Limit = pageSize;
            Offset = page * pageSize;
            SortColumn = sortColumn;
            SortDirection = sortDirection;
        }
    }
}


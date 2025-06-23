using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Models.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetArenaRankingQuery : IRequest<ArenaRankingDTO>
    {
        public ArenaRankingEnum Ranking { get; set; }

        public GetArenaRankingQuery(ArenaRankingEnum arenaRankingEnum)
        {
            Ranking = arenaRankingEnum;
        }
    }
}

using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Events
{
    public class ArenaRankingDTO
    {
        public Guid Id { get; set; }
        public ArenaRankingEnum Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ArenaRankingCompetitorDTO> Competitors { get; set; }
    }
}

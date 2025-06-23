using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Utils;
using System.ComponentModel.DataAnnotations;

namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public class ArenaRankingModel
    {
        public Guid Id { get; set; }
        public ArenaRankingEnum Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ArenaRankingCompetitorModel> Competitors { get; set; }

        private void UpdatePosition()
        {
            byte position = 0;

            var competitors = Competitors.OrderByDescending(c => c.Points);

            foreach (var competitor in competitors)
            {
                position++;
                competitor.SetPosition(position);
            }
        }

        public ArenaRankingCompetitorModel? GetRank(long tamerId)
        {
            UpdatePosition();

            var competitor = Competitors.FirstOrDefault(x => x.TamerId == tamerId);

            return competitor;
        }

        public void GetTop100()
        {
            if (Competitors != null)
            {
                Competitors.RemoveAll(c => c.Position > (byte)100);
            }
        }

        public void JoinRanking(long tamerId, int points)
        {
            var competitor = new ArenaRankingCompetitorModel(tamerId, points);
            Competitors.Add(competitor);
        }

        public long RemainingMinutes()
        {
            var remainingMinutes = (int)(EndDate - DateTime.Now).TotalMinutes;

            return UtilitiesFunctions.RemainingTimeMinutes(remainingMinutes);
        }
    }
}
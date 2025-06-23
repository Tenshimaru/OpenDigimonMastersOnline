using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Commons.DTOs.Events
{
    public class ArenaRankingCompetitorDTO
    {
        public Guid Id { get; set; }
        public DateTime InsertDate { get; set; }
        public long TamerId { get; set; }
        public int Points { get; set; }
        public byte Position { get; set; }
        public bool New { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public ArenaRankingDTO Ranking { get; set; }
        public Guid RankingId { get; set; }
    }
}

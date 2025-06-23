using DigitalWorldOnline.Commons.DTOs.Mechanics;
using Microsoft.EntityFrameworkCore;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Mechanics;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Arena;


namespace DigitalWorldOnline.Infrastructure
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<ArenaRankingDTO> ArenaRanking { get; set; }
        public DbSet<ArenaRankingCompetitorDTO> Competitor { get; set; }
     
        internal static void ArenaEntityConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ArenaRankingConfiguration());
            builder.ApplyConfiguration(new ArenaRankingCompetitorConfiguration());
        }
    }
}

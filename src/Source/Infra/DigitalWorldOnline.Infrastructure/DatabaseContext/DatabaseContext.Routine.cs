using Microsoft.EntityFrameworkCore;
using DigitalWorldOnline.Commons.DTOs.Routine;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Routine;

namespace DigitalWorldOnline.Infrastructure
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<RoutineDTO> Routine { get; set; }

        internal static void RoutineEntityConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new RoutineConfiguration());
        }
    }
}
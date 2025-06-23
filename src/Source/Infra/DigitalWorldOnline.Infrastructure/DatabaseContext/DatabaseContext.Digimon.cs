using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Digimon;
using Microsoft.EntityFrameworkCore;

namespace DigitalWorldOnline.Infrastructure
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<DigimonDTO> Digimon { get; set; }
        public DbSet<DigimonEvolutionDTO> DigimonEvolution { get; set; }
        public DbSet<DigimonDigicloneDTO> DigimonDigiclone { get; set; }
        public DbSet<DigimonBuffListDTO> DigimonBuffList { get; set; }
        public DbSet<DigimonLocationDTO> DigimonLocation { get; set; }

        internal static void DigimonEntityConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new DigimonConfiguration());
            builder.ApplyConfiguration(new DigimonDigicloneConfiguration());
            builder.ApplyConfiguration(new DigimonDigicloneHistoryConfiguration());
            builder.ApplyConfiguration(new DigimonEvolutionConfiguration());
            builder.ApplyConfiguration(new DigimonEvolutionSkillConfiguration());
            builder.ApplyConfiguration(new DigimonAttributeExperienceConfiguration());
            builder.ApplyConfiguration(new DigimonBuffListConfiguration());
            builder.ApplyConfiguration(new DigimonBuffConfiguration());
            builder.ApplyConfiguration(new DigimonLocationConfiguration());
        }
    }
}
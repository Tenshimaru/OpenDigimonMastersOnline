using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class KillSpawnConfigConfiguration : IEntityTypeConfiguration<KillSpawnConfigDTO>
    {
        public void Configure(EntityTypeBuilder<KillSpawnConfigDTO> builder)
        {
            builder
                .ToTable("KillSpawn", "Config")
                .HasKey(x => x.Id);

            builder
                .HasMany(x => x.SourceMobs)
                .WithOne(x => x.KillSpawn)
                .HasForeignKey(x => x.KillSpawnId);

            builder
                .HasMany(x => x.TargetMobs)
                .WithOne(x => x.KillSpawn)
                .HasForeignKey(x => x.KillSpawnId);
        }
    }
}
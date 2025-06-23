using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class KillSpawnSourceMobConfigConfiguration : IEntityTypeConfiguration<KillSpawnSourceMobConfigDTO>
    {
        public void Configure(EntityTypeBuilder<KillSpawnSourceMobConfigDTO> builder)
        {
            builder
                .ToTable("KillSpawnSourceMob", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.SourceMobType)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.SourceMobRequiredAmount)
                .HasColumnType("tinyint")
                .IsRequired();
        }
    }
}
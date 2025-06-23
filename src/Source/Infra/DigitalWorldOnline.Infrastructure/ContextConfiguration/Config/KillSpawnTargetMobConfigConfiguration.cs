using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class KillSpawnTargetMobConfigConfiguration : IEntityTypeConfiguration<KillSpawnTargetMobConfigDTO>
    {
        public void Configure(EntityTypeBuilder<KillSpawnTargetMobConfigDTO> builder)
        {
            builder
                .ToTable("KillSpawnTargetMob", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.TargetMobType)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.TargetMobAmount)
                .HasColumnType("tinyint")
                .IsRequired();

        }
    }
}
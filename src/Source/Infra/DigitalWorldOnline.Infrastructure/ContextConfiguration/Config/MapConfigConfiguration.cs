using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class MapConfigConfiguration : IEntityTypeConfiguration<MapConfigDTO>
    {
        public void Configure(EntityTypeBuilder<MapConfigDTO> builder)
        {
            builder
                .ToTable("Map", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.MapId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Name)
                .HasColumnType("varchar")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<MapTypeEnum, int>(
                    x => (int)x,
                    x => (MapTypeEnum)x))
                .HasDefaultValue(MapTypeEnum.Default)
                .IsRequired();

            builder
                .Property(e => e.Channels)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(e => e.MapRegionindex)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .HasMany(x => x.Mobs)
                .WithOne(x => x.GameMapConfig)
                .HasForeignKey(x => x.GameMapConfigId);

            builder
                .HasMany(x => x.KillSpawns)
                .WithOne(x => x.GameMapConfig)
                .HasForeignKey(x => x.GameMapConfigId);
        }
    }
}
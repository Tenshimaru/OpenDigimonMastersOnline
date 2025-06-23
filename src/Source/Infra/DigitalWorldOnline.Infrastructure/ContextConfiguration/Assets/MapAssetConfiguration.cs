using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class MapAssetConfiguration : IEntityTypeConfiguration<MapAssetDTO>
    {
        public void Configure(EntityTypeBuilder<MapAssetDTO> builder)
        {
            builder
                .ToTable("Map", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.MapId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property(e => e.RegionIndex)
                .HasColumnType("tinyint")
                .IsRequired();
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class MapRegionAssetConfiguration : IEntityTypeConfiguration<MapRegionAssetDTO>
    {
        public void Configure(EntityTypeBuilder<MapRegionAssetDTO> builder)
        {
            builder
                .ToTable("MapRegion", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Index)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.X)
                .HasColumnType("int")
                .HasDefaultValue(6500)
                .IsRequired();
            
            builder
                .Property(e => e.Y)
                .HasColumnType("int")
                .HasDefaultValue(6500)
                .IsRequired();
            
            builder
                .Property(e => e.Name)
                .HasColumnType("varchar")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
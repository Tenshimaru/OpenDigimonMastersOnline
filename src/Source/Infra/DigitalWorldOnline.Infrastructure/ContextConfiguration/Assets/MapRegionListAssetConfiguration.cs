using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class MapRegionListAssetConfiguration : IEntityTypeConfiguration<MapRegionListAssetDTO>
    {
        public void Configure(EntityTypeBuilder<MapRegionListAssetDTO> builder)
        {
            builder
                .ToTable("MapRegionList", "Asset")
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
                .HasMany(x => x.Regions)
                .WithOne(x => x.MapRegionList)
                .HasForeignKey(x => x.MapRegionListId);
        }
    }
}
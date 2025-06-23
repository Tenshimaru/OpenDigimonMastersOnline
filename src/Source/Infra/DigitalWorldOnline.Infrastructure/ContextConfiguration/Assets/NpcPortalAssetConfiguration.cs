using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class NpcPortalAssetConfiguration : IEntityTypeConfiguration<NpcPortalAssetDTO>
    {
        public void Configure(EntityTypeBuilder<NpcPortalAssetDTO> builder)
        {
            builder
                .ToTable("NpcPortal", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.PortalType)
                .HasColumnType("int")
                .IsRequired();
            builder
               .Property(e => e.PortalCount)
               .HasColumnType("int")
               .IsRequired();

            builder
                .HasMany(x => x.PortalsAsset)
                .WithOne(x => x.NpcAsset)
                .HasForeignKey(x => x.NpcAssetId);
        }
    }
}
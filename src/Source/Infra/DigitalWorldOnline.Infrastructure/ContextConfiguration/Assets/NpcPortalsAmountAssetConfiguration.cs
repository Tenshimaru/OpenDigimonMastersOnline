using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class NpcPortalsAmountAssetConfiguration : IEntityTypeConfiguration<NpcPortalsAmountAssetDTO>
    {
        public void Configure(EntityTypeBuilder<NpcPortalsAmountAssetDTO> builder)
        {
            builder
                .ToTable("NpcPortalsAmount", "Asset")
                .HasKey(x => x.Id);

            builder
              .HasMany(x => x.npcPortalsAsset)
              .WithOne(x => x.NpcAsset)
              .HasForeignKey(x => x.NpcAssetId);
        }
    }
}
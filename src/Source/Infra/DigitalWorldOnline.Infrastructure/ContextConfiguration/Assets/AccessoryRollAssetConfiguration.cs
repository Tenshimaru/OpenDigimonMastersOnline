using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class AccessoryRollAssetConfiguration : IEntityTypeConfiguration<AccessoryRollAssetDTO>
    {
        public void Configure(EntityTypeBuilder<AccessoryRollAssetDTO> builder)
        {
            builder
                .ToTable("AccessoryRoll", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.StatusAmount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RerollAmount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.Status)
                .WithOne(x => x.AccessoryRollAsset)
                .HasForeignKey(x => x.AccessoryRollAssetId);
        }
    }
}
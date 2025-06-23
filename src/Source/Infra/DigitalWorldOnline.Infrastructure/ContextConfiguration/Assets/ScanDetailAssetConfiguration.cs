using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ScanDetailAssetConfiguration : IEntityTypeConfiguration<ScanDetailAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ScanDetailAssetDTO> builder)
        {
            builder
                .ToTable("ScanDetail", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.MinAmount)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .Property(e => e.MaxAmount)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.ItemName)
                .HasColumnType("varchar")
                .HasMaxLength(100);

            builder
                .HasMany(x => x.Rewards)
                .WithOne(x => x.ScanDetailAsset)
                .HasForeignKey(x => x.ScanDetailAssetId);
        }
    }
}
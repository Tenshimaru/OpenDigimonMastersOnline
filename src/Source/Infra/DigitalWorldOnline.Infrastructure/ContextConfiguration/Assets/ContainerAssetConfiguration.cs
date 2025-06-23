using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ContainerAssetConfiguration : IEntityTypeConfiguration<ContainerAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ContainerAssetDTO> builder)
        {
            builder
                .ToTable("Container", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RewardAmount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.ItemName)
                .HasColumnType("varchar")
                .HasMaxLength(100);

            builder
                .HasMany(x => x.Rewards)
                .WithOne(x => x.ContainerAsset)
                .HasForeignKey(x => x.ContainerAssetId);
        }
    }
}
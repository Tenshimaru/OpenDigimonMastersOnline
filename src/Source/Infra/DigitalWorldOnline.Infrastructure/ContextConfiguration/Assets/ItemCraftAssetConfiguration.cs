using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ItemCraftAssetConfiguration : IEntityTypeConfiguration<ItemCraftAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ItemCraftAssetDTO> builder)
        {
            builder
                .ToTable("ItemCraft", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.SequencialId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.NpcId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.SuccessRate)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.Price)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(e => e.Amount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.Materials)
                .WithOne(x => x.ItemCraft)
                .HasForeignKey(x => x.ItemCraftId);
        }
    }
}
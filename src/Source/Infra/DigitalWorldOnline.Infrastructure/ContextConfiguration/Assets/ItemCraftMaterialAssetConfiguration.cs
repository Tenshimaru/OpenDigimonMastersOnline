using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ItemCraftMaterialAssetConfiguration : IEntityTypeConfiguration<ItemCraftMaterialAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ItemCraftMaterialAssetDTO> builder)
        {
            builder
                .ToTable("ItemCraftMaterial", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Amount)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
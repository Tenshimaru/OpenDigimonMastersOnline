using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class QuestSuppliesAssetConfiguration : IEntityTypeConfiguration<QuestSupplyAssetDTO>
    {
        public void Configure(EntityTypeBuilder<QuestSupplyAssetDTO> builder)
        {
            builder
                .ToTable("QuestSupply", "Asset")
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
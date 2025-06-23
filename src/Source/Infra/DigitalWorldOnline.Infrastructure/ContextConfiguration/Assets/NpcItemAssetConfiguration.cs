using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class NpcItemAssetConfiguration : IEntityTypeConfiguration<NpcItemAssetDTO>
    {
        public void Configure(EntityTypeBuilder<NpcItemAssetDTO> builder)
        {
            builder
                .ToTable("NpcItem", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
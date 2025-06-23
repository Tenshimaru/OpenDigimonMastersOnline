using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class GotchaRareItemsAssetConfiguration : IEntityTypeConfiguration<GotchaRareItemsAssetDTO>
    {
        public void Configure(EntityTypeBuilder<GotchaRareItemsAssetDTO> builder)
        {
            builder
                .ToTable("GotchaRareItems", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.GotchaId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RareItem)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RareItemCnt)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(e => e.Name)
                .HasColumnType("text")
                .IsRequired();

            builder
                .Property(e => e.RareItemGive)
                .HasColumnType("smallint")
                .IsRequired();
        }
    }
}
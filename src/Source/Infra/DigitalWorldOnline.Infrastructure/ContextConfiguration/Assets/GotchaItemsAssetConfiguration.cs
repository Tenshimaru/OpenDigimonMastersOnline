using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class GotchaItemsAssetConfiguration : IEntityTypeConfiguration<GotchaItemsAssetDTO>
    {
        public void Configure(EntityTypeBuilder<GotchaItemsAssetDTO> builder)
        {
            builder
                .ToTable("GotchaItems", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.GotchaId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.ItemCount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Name)
                .HasColumnType("text")
                .IsRequired();

            builder
                .Property(e => e.InitialQuanty)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Quanty)
                .HasColumnType("int")
                .IsRequired();

        }
    }
}
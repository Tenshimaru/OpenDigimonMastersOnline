using DigitalWorldOnline.Commons.DTOs.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Shop
{
    public class ConsignedShopConfiguration : IEntityTypeConfiguration<ConsignedShopDTO>
    {
        public void Configure(EntityTypeBuilder<ConsignedShopDTO> builder)
        {
            builder
                .ToTable("ConsignedShop", "Shop")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.ShopName)
                .HasColumnType("varchar")
                .HasMaxLength(30)
                .IsRequired();

            builder
                .Property(x => x.Channel)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .Property(x => x.ItemId)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.GeneralHandler)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasOne(x => x.Location)
                .WithOne(x => x.ConsignedShop)
                .HasForeignKey<ConsignedShopLocationDTO>(x => x.ConsignedShopId);
        }
    }
}
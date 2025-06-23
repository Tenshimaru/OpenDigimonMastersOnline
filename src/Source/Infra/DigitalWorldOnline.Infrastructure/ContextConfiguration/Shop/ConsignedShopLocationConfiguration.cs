using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.DTOs.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Shop
{
    public class ConsignedShopLocationConfiguration : IEntityTypeConfiguration<ConsignedShopLocationDTO>
    {
        public void Configure(EntityTypeBuilder<ConsignedShopLocationDTO> builder)
        {
            builder
                .ToTable("Location", "Shop")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.MapId)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.X)
                .HasColumnType("int")
                .HasDefaultValue(5000)
                .IsRequired();

            builder
                .Property(x => x.Y)
                .HasColumnType("int")
                .HasDefaultValue(4500)
                .IsRequired();

            builder
                .Property(x => x.Z)
                 .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();

        }
    }
}
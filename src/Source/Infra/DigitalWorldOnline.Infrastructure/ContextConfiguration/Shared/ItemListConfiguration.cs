using DigitalWorldOnline.Commons.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Shared
{
    public class ItemListConfiguration : IEntityTypeConfiguration<ItemListDTO>
    {
        public void Configure(EntityTypeBuilder<ItemListDTO> builder)
        {
            builder
                .ToTable("ItemList", "Shared")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Type)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<ItemListEnum, int>(
                    x => (int)x,
                    x => (ItemListEnum)x))
                .IsRequired();

            builder
                .Property(x => x.Size)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(x => x.Bits)
                .HasColumnType("bigint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .HasMany(x => x.Items)
                .WithOne(x => x.ItemList)
                .HasForeignKey(x => x.ItemListId);
        }
    }
}
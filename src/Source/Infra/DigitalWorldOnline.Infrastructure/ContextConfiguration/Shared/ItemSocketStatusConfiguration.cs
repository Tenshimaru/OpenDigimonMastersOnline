using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Shared
{
    public class ItemSocketStatusConfiguration : IEntityTypeConfiguration<ItemSocketStatusDTO>
    {
        public void Configure(EntityTypeBuilder<ItemSocketStatusDTO> builder)
        {
            builder
                .ToTable("ItemSocketStatus", "Shared")
                .HasKey(x => x.Id);

            builder
                   .Property(x => x.AttributeId)
                   .HasColumnType("smallint")
                   .IsRequired();

            builder
                .Property(x => x.Type)
                .HasColumnType("smallint")
                .HasConversion(new ValueConverter<AccessoryStatusTypeEnum, short>(
                    x => (short)x,
                    x => (AccessoryStatusTypeEnum)x))
                .IsRequired();

            builder
                .Property(x => x.Value)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(x => x.Slot)
                .HasColumnType("tinyint")
                .IsRequired();
        }
    }
}
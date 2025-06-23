using DigitalWorldOnline.Commons.DTOs.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Shared
{
    public class ItemConfiguration : IEntityTypeConfiguration<ItemDTO>
    {
        public void Configure(EntityTypeBuilder<ItemDTO> builder)
        {
            builder
                .ToTable("Item", "Shared")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.Amount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.Power)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(x => x.RerollLeft)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
               .Property(x => x.Duration)
               .HasColumnType("int")
               .IsRequired();

            builder
               .Property(x => x.EndDate)
               .HasColumnType("datetime2")
               .HasDefaultValueSql("getdate()")
               .IsRequired();

            builder
                .Property(x => x.FirstExpired)
                .HasColumnType("bit")
                .HasDefaultValue(1)
                .IsRequired();

            builder
            .Property(x => x.FamilyType)
            .HasColumnType("tinyint")
            .IsRequired();

            builder
                .Property(x => x.Slot)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.AccessoryStatus)
                .WithOne(x => x.Item)
                .HasForeignKey(x => x.ItemId);
        }
    }
}
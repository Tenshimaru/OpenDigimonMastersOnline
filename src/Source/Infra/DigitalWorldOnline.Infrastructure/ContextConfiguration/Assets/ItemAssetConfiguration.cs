using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ItemAssetConfiguration : IEntityTypeConfiguration<ItemAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ItemAssetDTO> builder)
        {
            builder
                .ToTable("ItemInfo", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Name)
                .HasColumnType("varchar")
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(e => e.Class)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.TypeN)
                .HasColumnType("int")
                .IsRequired();

            builder
              .Property(e => e.ApplyValueMin)
              .HasColumnType("smallint")
              .IsRequired();

            builder
             .Property(e => e.ApplyValueMax)
             .HasColumnType("smallint")
             .IsRequired();

            builder
             .Property(e => e.ApplyElement)
             .HasColumnType("smallint")
             .IsRequired();

            builder
                .Property(e => e.Section)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.SellType)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.BoundType)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.UseTimeType)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.SkillCode)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(e => e.TamerMinLevel)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.TamerMaxLevel)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.DigimonMinLevel)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.DigimonMaxLevel)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.SellPrice)
                .HasColumnType("bigint")
                .IsRequired();
            
            builder
                .Property(e => e.ScanPrice)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.DigicorePrice)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.EventPriceId)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.EventPriceAmount)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.UsageTimeMinutes)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Overlap)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(x => x.Target)
                .HasConversion(new ValueConverter<ItemConsumeTargetEnum, int>(
                    x => (int)x,
                    x => (ItemConsumeTargetEnum)x))
                .HasColumnType("int")
                .HasDefaultValue(ItemConsumeTargetEnum.Unavailable)
                .IsRequired();
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class QuestAssetConfiguration : IEntityTypeConfiguration<QuestAssetDTO>
    {
        public void Configure(EntityTypeBuilder<QuestAssetDTO> builder)
        {
            builder
                .ToTable("Quest", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.QuestId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.QuestType)
                .HasConversion(new ValueConverter<QuestTypeEnum, int>(
                    x => (int)x,
                    x => (QuestTypeEnum)x))
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.TargetType)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.TargetValue)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.QuestSupplies)
                .WithOne(x => x.Quest)
                .HasForeignKey(x => x.QuestId);
            
            builder
                .HasMany(x => x.QuestConditions)
                .WithOne(x => x.Quest)
                .HasForeignKey(x => x.QuestId);
            
            builder
                .HasMany(x => x.QuestGoals)
                .WithOne(x => x.Quest)
                .HasForeignKey(x => x.QuestId);
            
            builder
                .HasMany(x => x.QuestRewards)
                .WithOne(x => x.Quest)
                .HasForeignKey(x => x.QuestId);
            
            builder
                .HasMany(x => x.QuestEvents)
                .WithOne(x => x.Quest)
                .HasForeignKey(x => x.QuestId);
        }
    }
}
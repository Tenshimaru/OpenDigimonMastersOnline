using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class QuestGoalsAssetConfiguration : IEntityTypeConfiguration<QuestGoalAssetDTO>
    {
        public void Configure(EntityTypeBuilder<QuestGoalAssetDTO> builder)
        {
            builder
                .ToTable("QuestGoal", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.GoalType)
                .HasConversion(new ValueConverter<QuestGoalTypeEnum, int>(
                    x => (int)x,
                    x => (QuestGoalTypeEnum)x))
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.GoalId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.GoalAmount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.CurTypeCount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.SubValue)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.SubValueTwo)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
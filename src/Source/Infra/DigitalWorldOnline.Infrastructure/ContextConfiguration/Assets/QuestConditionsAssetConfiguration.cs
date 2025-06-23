using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class QuestConditionsAssetConfiguration : IEntityTypeConfiguration<QuestConditionAssetDTO>
    {
        public void Configure(EntityTypeBuilder<QuestConditionAssetDTO> builder)
        {
            builder
                .ToTable("QuestCondition", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ConditionType)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.ConditionId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.ConditionCount)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
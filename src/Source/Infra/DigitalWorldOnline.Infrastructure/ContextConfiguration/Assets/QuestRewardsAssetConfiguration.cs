using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class QuestRewardsAssetConfiguration : IEntityTypeConfiguration<QuestRewardAssetDTO>
    {
        public void Configure(EntityTypeBuilder<QuestRewardAssetDTO> builder)
        {
            builder
                .ToTable("QuestReward", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Reward)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RewardType)
                .HasConversion(new ValueConverter<QuestRewardTypeEnum, int>(
                    x => (int)x,
                    x => (QuestRewardTypeEnum)x))
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.RewardObjectList)
                .WithOne(x => x.QuestReward)
                .HasForeignKey(x => x.QuestRewardId);
        }
    }
}
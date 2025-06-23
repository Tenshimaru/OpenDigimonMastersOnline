using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ArenaRankingDailyItemRewardsConfiguration : IEntityTypeConfiguration<ArenaRankingDailyItemRewardsDTO>
    {
        public void Configure(EntityTypeBuilder<ArenaRankingDailyItemRewardsDTO> builder)
        {
            builder
               .ToTable("ArenaDailyItemRewards", "Asset")
               .HasKey(x => x.Id);

            builder
                .Property(e => e.WeekDay)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DayOfWeek, int>(
            x => (int)x,
                    x => (DayOfWeek)x))
                .IsRequired();

            builder
                 .HasMany(x => x.Rewards)
                 .WithOne(x => x.Reward)
                 .HasForeignKey(x => x.RewardId);

        }
    }
}
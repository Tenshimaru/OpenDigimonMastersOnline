using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ArenaRankingDailyItemRewardConfiguration : IEntityTypeConfiguration<ArenaRankingDailyItemRewardDTO>
    {
        public void Configure(EntityTypeBuilder<ArenaRankingDailyItemRewardDTO> builder)
        {
            builder
                .ToTable("ArenaDailyItemReward", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Amount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RequiredCoins)
                .HasColumnType("tinyint")
                .IsRequired();          
        }
    }
}
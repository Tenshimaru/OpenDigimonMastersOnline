using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Models.Asset;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class TimeRewardAssetConfiguration : IEntityTypeConfiguration<TimeRewardAssetDTO>
    {
        public void Configure(EntityTypeBuilder<TimeRewardAssetDTO> builder)
        {

            builder
                .ToTable("TimeEvent", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(x => x.CurrentReward)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.ItemCount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.RewardIndex)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
        }
    }
}
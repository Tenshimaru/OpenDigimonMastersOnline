using DigitalWorldOnline.Commons.DTOs.Config.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.Events
{
    public class EventMobDropRewardConfigConfiguration : IEntityTypeConfiguration<EventMobDropRewardConfigDTO>
    {
        public void Configure(EntityTypeBuilder<EventMobDropRewardConfigDTO> builder)
        {
            builder
                .ToTable("EventMobDropReward", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.MinAmount)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .Property(e => e.MaxAmount)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .HasMany(x => x.Drops)
                .WithOne(x => x.DropReward)
                .HasForeignKey(x => x.DropRewardId);

            builder
                .HasOne(x => x.BitsDrop)
                .WithOne(x => x.DropReward)
                .HasForeignKey<EventBitsDropConfigDTO>(x => x.DropRewardId);
        }
    }
}
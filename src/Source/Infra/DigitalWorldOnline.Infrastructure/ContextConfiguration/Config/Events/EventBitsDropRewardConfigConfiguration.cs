using DigitalWorldOnline.Commons.DTOs.Config.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.Events
{
    public class EventBitsDropRewardConfigConfiguration : IEntityTypeConfiguration<EventBitsDropConfigDTO>
    {
        public void Configure(EntityTypeBuilder<EventBitsDropConfigDTO> builder)
        {
            builder
                .ToTable("EventBitsDropReward", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.MinAmount)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.MaxAmount)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.Chance)
                .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.Events
{
    public class EventItemDropRewardConfigConfiguration : IEntityTypeConfiguration<EventItemDropConfigDTO>
    {
        public void Configure(EntityTypeBuilder<EventItemDropConfigDTO> builder)
        {
            builder
                .ToTable("EventItemDropReward", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
            
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

            builder
                .Property(e => e.Rank)
                .HasColumnType("int")
                .HasDefaultValue(1)
                .IsRequired();
        }
    }
}
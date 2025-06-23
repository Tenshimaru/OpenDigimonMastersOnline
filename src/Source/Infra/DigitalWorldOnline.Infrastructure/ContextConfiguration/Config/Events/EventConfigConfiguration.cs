using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.Events
{
    public class EventConfigConfiguration : IEntityTypeConfiguration<EventConfigDTO>
    {
        public void Configure(EntityTypeBuilder<EventConfigDTO> builder)
        {
            builder
                .ToTable("Events", "Config")
                .HasKey(e => e.Id);

            // Configure properties
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.IsEnabled)
                .IsRequired();
            
            builder
                .Property(e => e.Rounds)
                .HasColumnType("tinyint")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(e => e.StartDay)
                .HasConversion(new ValueConverter<EventStartDayEnum, int>(
                    x => (int)x,
                    x => (EventStartDayEnum)x))
                .HasColumnType("int")
                .HasDefaultValue(EventStartDayEnum.Everyday)
                .IsRequired();

            builder.Property(e => e.StartsAt)
                .IsRequired();

            // Configure relations
            builder.HasMany(e => e.EventMaps)
                .WithOne()
                .HasForeignKey(x => x.EventConfigId);
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.Events
{
    public class EventMapsConfigConfiguration : IEntityTypeConfiguration<EventMapsConfigDTO>
    {
        public void Configure(EntityTypeBuilder<EventMapsConfigDTO> builder)
        {
            builder
                .ToTable("EventMaps", "Config")
                .HasKey(e => e.Id);

            // Configure properties
            builder.Property(e => e.EventConfigId)
                .IsRequired();

            builder.Property(e => e.MapId)
                .IsRequired();

            builder.Property(e => e.Channels)
                .IsRequired();

            builder.Property(e => e.IsEnabled)
                .IsRequired();

            // Configure relations
            builder.HasOne(e => e.Map)
                .WithMany()
                .HasForeignKey(e => e.MapId)
                .HasPrincipalKey(x => x.MapId);

            builder.HasOne(e => e.EventConfig)
                .WithMany(ec => ec.EventMaps)
                .HasForeignKey(e => e.EventConfigId);

            // Additional configurations can be added as necessary
        }
    }
}
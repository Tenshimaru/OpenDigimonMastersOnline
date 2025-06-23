using DigitalWorldOnline.Commons.DTOs.Config.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.Events
{
    public class EventMobLocationConfigConfiguration : IEntityTypeConfiguration<EventMobLocationConfigDTO>
    {
        public void Configure(EntityTypeBuilder<EventMobLocationConfigDTO> builder)
        {
            builder
                .ToTable("EventMobLocation", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.MapId)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.X)
                .HasColumnType("int")
                .HasDefaultValue(5000)
                .IsRequired();

            builder
                .Property(x => x.Y)
                .HasColumnType("int")
                .HasDefaultValue(4500)
                .IsRequired();

            builder
                .Property(x => x.Z)
                .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Event
{
    public class AttendanceRewardConfiguration : IEntityTypeConfiguration<AttendanceRewardDTO>
    {
        public void Configure(EntityTypeBuilder<AttendanceRewardDTO> builder)
        {
            builder
                .ToTable("AttendanceReward", "Event")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.TotalDays)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();
        }
    }
}
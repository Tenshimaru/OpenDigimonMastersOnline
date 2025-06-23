using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Event
{
    public class TimeRewardConfiguration : IEntityTypeConfiguration<TimeRewardDTO>
    {
        public void Configure(EntityTypeBuilder<TimeRewardDTO> builder)
        {
            builder
                .ToTable("TimeReward", "Event")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.RewardIndex)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<TimeRewardIndexEnum, int>(
                    x => (int)x,
                    x => (TimeRewardIndexEnum)x))
                .HasDefaultValue(TimeRewardIndexEnum.First)
                .IsRequired();

            builder
                .Property(x => x.StartTime)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .Property(x => x.AtualTime)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
        }
    }
}
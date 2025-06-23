using DigitalWorldOnline.Commons.DTOs.Routine;
using DigitalWorldOnline.Commons.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Routine
{
    public class RoutineConfiguration : IEntityTypeConfiguration<RoutineDTO>
    {
        public void Configure(EntityTypeBuilder<RoutineDTO> builder)
        {
            builder
                .ToTable("Routine", "Routine")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Type)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<RoutineTypeEnum, int>(
                    x => (int)x,
                    x => (RoutineTypeEnum)x))
                .IsRequired();

            builder
                .Property(x => x.Active)
                .HasColumnType("bit")
                .IsRequired();
            
            builder
                .Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(x => x.Interval)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.NextRunTime)
                .HasColumnType("datetime2")
                .IsRequired();

            builder
                .Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.HasData(
                new RoutineDTO()
                {
                    Id = 1,
                    CreatedAt = DateTime.Now,
                    Active = true,
                    Interval = 1,
                    Name = "Daily Quests",
                    Type = RoutineTypeEnum.DailyQuests,
                    NextRunTime = DateTime.Now.AddDays(1).Date
                }
            );
        }
    }
}
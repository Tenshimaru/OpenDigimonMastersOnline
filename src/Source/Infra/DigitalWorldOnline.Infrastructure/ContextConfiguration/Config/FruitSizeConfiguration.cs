using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class FruitSizeConfiguration : IEntityTypeConfiguration<FruitSizeConfigDTO>
    {
        public void Configure(EntityTypeBuilder<FruitSizeConfigDTO> builder)
        {
            builder
                .ToTable("FruitSize", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.HatchGrade)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonHatchGradeEnum, int>(
                    x => (int)x,
                    x => (DigimonHatchGradeEnum)x))
                .IsRequired();

            builder
                .Property(x => x.Size)
                 .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.MinSize)
                 .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.MaxSize)
                 .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.Chance)
                 .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();

        }
    }
}
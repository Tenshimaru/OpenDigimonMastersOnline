using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class HatchConfiguration : IEntityTypeConfiguration<HatchConfigDTO>
    {
        public void Configure(EntityTypeBuilder<HatchConfigDTO> builder)
        {
            builder
                .ToTable("Hatch", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Type)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<HatchTypeEnum, int>(
                    x => (int)x,
                    x => (HatchTypeEnum)x))
                .IsRequired();

            builder
                .Property(e => e.SuccessChance)
                 .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();


            builder
                .Property(e => e.BreakChance)
                .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();

        }
    }
}
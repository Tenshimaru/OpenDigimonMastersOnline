using DigitalWorldOnline.Commons.DTOs.Digimon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Digimon
{
    public class DigimonBuffConfiguration : IEntityTypeConfiguration<DigimonBuffDTO>
    {
        public void Configure(EntityTypeBuilder<DigimonBuffDTO> builder)
        {
            builder
                .ToTable("Buff", "Digimon")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.BuffId)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.Duration)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.Cooldown)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
               .Property(x => x.TypeN)
               .HasColumnType("int")
               .HasDefaultValue(0)
               .IsRequired();

            builder
                .Property(x => x.SkillId)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.EndDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .Property(x => x.CoolEndDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();
        }
    }
}
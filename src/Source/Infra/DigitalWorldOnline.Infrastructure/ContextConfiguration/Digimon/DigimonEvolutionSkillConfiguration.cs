using DigitalWorldOnline.Commons.DTOs.Digimon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Digimon
{
    public class DigimonEvolutionSkillConfiguration : IEntityTypeConfiguration<DigimonEvolutionSkillDTO>
    {
        public void Configure(EntityTypeBuilder<DigimonEvolutionSkillDTO> builder)
        {
            builder
                .ToTable("EvolutionSkill", "Digimon")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.CurrentLevel)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .Property(x => x.EndDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
               .Property(x => x.Duration)
               .HasColumnType("int")
               .HasDefaultValue(0)
               .IsRequired();

            builder
                .Property(x => x.MaxLevel)
                .HasColumnType("tinyint")
                .HasDefaultValue(10)
                .IsRequired();


        }
    }
}
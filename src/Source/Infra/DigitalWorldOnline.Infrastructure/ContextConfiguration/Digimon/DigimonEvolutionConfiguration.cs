using DigitalWorldOnline.Commons.DTOs.Digimon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Digimon
{
    public class DigimonEvolutionConfiguration : IEntityTypeConfiguration<DigimonEvolutionDTO>
    {
        public void Configure(EntityTypeBuilder<DigimonEvolutionDTO> builder)
        {
            builder
                .ToTable("Evolution", "Digimon")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Unlocked)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .Property(x => x.SkillPoints)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();
            
            builder
                .Property(x => x.SkillMastery)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();
            
            builder
                .Property(x => x.SkillExperience)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
            
            builder
                .Property(x => x.Type)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.Skills)
                .WithOne(x => x.Evolution)
                .HasForeignKey(x => x.EvolutionId);
        }
    }
}
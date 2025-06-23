using DigitalWorldOnline.Commons.DTOs.Digimon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Digimon
{
    public class DigimonAttributeExperienceConfiguration : IEntityTypeConfiguration<DigimonAttributeExperienceDTO>
    {
        public void Configure(EntityTypeBuilder<DigimonAttributeExperienceDTO> builder)
        {
            builder
                .ToTable("AttributeExperience", "Digimon")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Data)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Vaccine)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Virus)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Ice)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Water)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Fire)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Land)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Wind)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Wood)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Light)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Dark)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Thunder)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(x => x.Steel)
                .HasColumnType("smallint")
                .IsRequired();
        }
    }
}
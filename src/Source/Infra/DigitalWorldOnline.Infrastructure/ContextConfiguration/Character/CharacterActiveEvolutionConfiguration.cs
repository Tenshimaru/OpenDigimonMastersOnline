using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterActiveEvolutionConfiguration : IEntityTypeConfiguration<CharacterActiveEvolutionDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterActiveEvolutionDTO> builder)
        {
            builder
                .ToTable("ActiveEvolution", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.DsPerSecond)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.XgPerSecond)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
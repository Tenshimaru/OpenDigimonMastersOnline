using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterSealConfiguration : IEntityTypeConfiguration<CharacterSealDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterSealDTO> builder)
        {
            builder
                .ToTable("Seal", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.SealId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.Amount)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(x => x.Amount)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.Favorite)
                .HasColumnType("bit")
                .IsRequired();

            builder
                .Property(x => x.SequentialId)
                .HasColumnType("smallint")
                .IsRequired();
        }
    }
}
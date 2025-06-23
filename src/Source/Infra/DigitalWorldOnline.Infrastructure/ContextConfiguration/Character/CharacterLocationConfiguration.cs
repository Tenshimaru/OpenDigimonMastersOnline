using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterLocationConfiguration : IEntityTypeConfiguration<CharacterLocationDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterLocationDTO> builder)
        {
            builder
                .ToTable("Location", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.MapId)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.X)
                .HasColumnType("int")
                .HasDefaultValue(5000)
                .IsRequired();

            builder
                .Property(x => x.Y)
                .HasColumnType("int")
                .HasDefaultValue(4500)
                .IsRequired();

            builder
                .Property(x => x.Z)
               .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();

        }
    }
}
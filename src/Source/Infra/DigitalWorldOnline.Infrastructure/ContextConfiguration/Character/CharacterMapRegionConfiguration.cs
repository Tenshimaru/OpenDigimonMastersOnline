using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterMapRegionConfiguration : IEntityTypeConfiguration<CharacterMapRegionDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterMapRegionDTO> builder)
        {
            builder
                .ToTable("MapRegion", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Unlocked)
                .HasColumnType("tinyint")
                .HasDefaultValue(0)
                .IsRequired();
        }
    }
}
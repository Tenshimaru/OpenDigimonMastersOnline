using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterDigimonArchiveItemConfiguration : IEntityTypeConfiguration<CharacterDigimonArchiveItemDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterDigimonArchiveItemDTO> builder)
        {
            builder
                .ToTable("DigimonArchiveItem", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Slot)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.DigimonId)
                .HasColumnType("bigint")
                .IsRequired();
        }
    }
}
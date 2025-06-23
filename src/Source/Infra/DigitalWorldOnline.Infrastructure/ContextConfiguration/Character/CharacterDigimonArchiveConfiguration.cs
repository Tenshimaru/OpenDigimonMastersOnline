using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterDigimonArchiveConfiguration : IEntityTypeConfiguration<CharacterDigimonArchiveDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterDigimonArchiveDTO> builder)
        {
            builder
                .ToTable("DigimonArchive", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Slots)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.DigimonArchives)
                .WithOne(x => x.DigimonArchive)
                .HasForeignKey(x => x.DigimonArchiveId);
        }
    }
}
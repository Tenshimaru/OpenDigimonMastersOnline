using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterSealListConfiguration : IEntityTypeConfiguration<CharacterSealListDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterSealListDTO> builder)
        {
            builder
                .ToTable("SealList", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.SealLeaderId)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .HasMany(x => x.Seals)
                .WithOne(x => x.SealList)
                .HasForeignKey(x => x.SealListId);
        }
    }
}
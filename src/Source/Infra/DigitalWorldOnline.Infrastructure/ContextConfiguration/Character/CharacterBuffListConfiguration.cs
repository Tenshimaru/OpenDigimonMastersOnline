using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterBuffListConfiguration : IEntityTypeConfiguration<CharacterBuffListDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterBuffListDTO> builder)
        {
            builder
                .ToTable("BuffList", "Character")
                .HasKey(x => x.Id);

            builder
                .HasMany(x => x.Buffs)
                .WithOne(x => x.BuffList)
                .HasForeignKey(x => x.BuffListId);
        }
    }
}
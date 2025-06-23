using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterFoeConfiguration : IEntityTypeConfiguration<CharacterFoeDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterFoeDTO> builder)
        {
            builder
                .ToTable("Foe", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(25)
                .IsRequired();

            builder
                .Property(x => x.Annotation)
                .HasColumnType("varchar")
                .HasMaxLength(25)
                .IsRequired();
            
            builder
                .Property(x => x.FoeId)
                .HasColumnType("bigint")
                .IsRequired();
            
            builder
                .Property(x => x.CharacterId)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .HasOne(x => x.Character)
                .WithMany(x => x.Foes)
                .HasForeignKey(x => x.CharacterId);

            builder
                .HasOne(x => x.Foe)
                .WithMany(x => x.Foed)
                .HasForeignKey(x => x.FoeId);
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterEncyclopediaEvolutionsConfiguration : IEntityTypeConfiguration<CharacterEncyclopediaEvolutionsDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterEncyclopediaEvolutionsDTO> builder)
        {
            builder
                .ToTable("EncyclopediaEvolutions", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.CharacterEncyclopediaId)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(x => x.DigimonBaseType)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.SlotLevel)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.IsUnlocked)
                .HasColumnType("bit")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(x => x.CreateDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();


            builder
                .HasOne(c => c.Encyclopedia)
                .WithMany(x => x.Evolutions)
                .HasForeignKey(x => x.CharacterEncyclopediaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.BaseInfo)
                .WithMany()
                .HasForeignKey(x => x.DigimonBaseType)
                .HasPrincipalKey(x => x.Type)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
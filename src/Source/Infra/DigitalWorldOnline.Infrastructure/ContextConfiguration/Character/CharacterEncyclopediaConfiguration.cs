using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterEncyclopediaConfiguration : IEntityTypeConfiguration<CharacterEncyclopediaDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterEncyclopediaDTO> builder)
        {
            builder
                .ToTable("Encyclopedia", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.CharacterId)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(x => x.DigimonEvolutionId)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(x => x.Level)
                .HasColumnType("int")
                .HasDefaultValue(1)
                .IsRequired();

            builder
                .Property(x => x.Size)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.EnchantAT)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.EnchantBL)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.EnchantCT)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.EnchantEV)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.EnchantHP)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.IsRewardAllowed)
                .HasColumnType("bit")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(x => x.IsRewardReceived)
                .HasColumnType("bit")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(x => x.CreateDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .HasOne(c => c.Character)
                .WithMany(x => x.Encyclopedia)
                .HasForeignKey(x => x.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(c => c.EvolutionAsset)
                .WithMany()
                .HasForeignKey(x => x.DigimonEvolutionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(c => c.Evolutions)
                .WithOne(c => c.Encyclopedia)
                .HasForeignKey(x => x.CharacterEncyclopediaId);
        }
    }
}
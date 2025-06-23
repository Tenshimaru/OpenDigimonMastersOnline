using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class CharacterLevelStatusAssetConfiguration : IEntityTypeConfiguration<CharacterLevelStatusAssetDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterLevelStatusAssetDTO> builder)
        {
            builder
                .ToTable("CharacterLevelStatus", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Type)
                .HasConversion(new ValueConverter<CharacterModelEnum, int>(
                    x => (int)x,
                    x => (CharacterModelEnum)x))
                .HasColumnType("int")
                .HasDefaultValue(CharacterModelEnum.MarcusDamon)
                .IsRequired();
            
            builder
                .Property(e => e.Level)
                .HasColumnType("tinyint")
                .HasDefaultValue(1)
                .IsRequired();

            builder
                .Property(e => e.ExpValue)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(e => e.ARValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.ASValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.ATValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.BLValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.CTValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.DEValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.DSValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.EVValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.HPValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.HTValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.MSValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.WSValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
        }
    }
}
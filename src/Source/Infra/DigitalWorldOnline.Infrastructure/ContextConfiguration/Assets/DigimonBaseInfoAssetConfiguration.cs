using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class DigimonBaseInfoAssetConfiguration : IEntityTypeConfiguration<DigimonBaseInfoAssetDTO>
    {
        public void Configure(EntityTypeBuilder<DigimonBaseInfoAssetDTO> builder)
        {
            builder
                .ToTable("DigimonBaseInfo", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Model)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();
            
            builder
                .Property(e => e.Level)
                .HasColumnType("tinyint")
                .HasDefaultValue(1)
                .IsRequired();
            
            builder
                .Property(e => e.ScaleType)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();
            
            builder
                .Property(e => e.EvolutionType)
                .HasColumnType("int")
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

            builder
                .Property(x => x.Attribute)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonAttributeEnum, int>(
                    x => (int)x,
                    x => (DigimonAttributeEnum)x))
                .HasDefaultValue(DigimonAttributeEnum.None)
                .IsRequired();

            builder
                .Property(x => x.Element)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonElementEnum, int>(
                    x => (int)x,
                    x => (DigimonElementEnum)x))
                .HasDefaultValue(DigimonElementEnum.Neutral)
                .IsRequired();

            builder
                .Property(x => x.Family1)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonFamilyEnum, int>(
                    x => (int)x,
                    x => (DigimonFamilyEnum)x))
                .HasDefaultValue(DigimonFamilyEnum.None)
                .IsRequired();

            builder
                .Property(x => x.Family2)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonFamilyEnum, int>(
                    x => (int)x,
                    x => (DigimonFamilyEnum)x))
                .HasDefaultValue(DigimonFamilyEnum.None)
                .IsRequired();

            builder
                .Property(x => x.Family3)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonFamilyEnum, int>(
                    x => (int)x,
                    x => (DigimonFamilyEnum)x))
                .HasDefaultValue(DigimonFamilyEnum.None)
                .IsRequired();
        }
    }
}
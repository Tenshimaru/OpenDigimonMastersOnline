using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class DeckBookInfoConfiguration : IEntityTypeConfiguration<DeckBookInfoAssetDTO>
    {
        public void Configure(EntityTypeBuilder<DeckBookInfoAssetDTO> builder)
        {
            builder
                .ToTable("DeckBookInfo", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.OptionId)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Type)
                .HasConversion(new ValueConverter<DeckBookInfoTypesEnum, int>(
                    x => (int)x,
                    x => (DeckBookInfoTypesEnum)x))
                .HasColumnType("int")
                .HasDefaultValue(DeckBookInfoTypesEnum.None)
                .IsRequired();
            
            builder
                .Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnType("varchar")
                .IsRequired();

            builder
                .Property(e => e.Explain)
                .HasMaxLength(200)
                .HasColumnType("varchar")
                .IsRequired();

            builder
                .HasMany(x => x.Options)
                .WithOne(x => x.DeckBookInfo)
                .HasForeignKey(x => x.OptionId)
                .HasPrincipalKey(x => x.OptionId);

            builder
                .HasData(
                    new DeckBookInfoAssetDTO
                    {
                        Id = 1,
                        OptionId = 0,
                        Type= DeckBookInfoTypesEnum.None,
                        Name = "None",
                        Explain = "None."
                    },
                    new DeckBookInfoAssetDTO
                    {
                        Id = 2,
                        OptionId = 1,
                        Type= DeckBookInfoTypesEnum.AT,
                        Name = "Additional damage to attack",
                        Explain = "Additional damage to normal attack is applied."
                    },
                    new DeckBookInfoAssetDTO
                    {
                        Id = 3,
                        OptionId = 2,
                        Type= DeckBookInfoTypesEnum.SK,
                        Name = "Additional damage to skill",
                        Explain = "Additional damage to skill attack is applied."
                    },
                    new DeckBookInfoAssetDTO
                    {
                        Id = 4,
                        OptionId = 3,
                        Type= DeckBookInfoTypesEnum.CD,
                        Name = "Increase critical hit damage",
                        Explain = "Increase critical hit damage."
                    },
                    new DeckBookInfoAssetDTO
                    {
                        Id = 5,
                        OptionId = 4,
                        Type= DeckBookInfoTypesEnum.SC,
                        Name = "Reset skill cooldown time",
                        Explain = "Reset skills' cooldown time when activated."
                    },
                    new DeckBookInfoAssetDTO
                    {
                        Id = 6,
                        OptionId = 5,
                        Type= DeckBookInfoTypesEnum.HP,
                        Name = "Increase Max HP",
                        Explain = "Increase digimon's Max HP."
                    },
                    new DeckBookInfoAssetDTO
                    {
                        Id = 7,
                        OptionId = 6,
                        Type= DeckBookInfoTypesEnum.AS,
                        Name = "Increase attack speed",
                        Explain = "Increase normal attacking speed."
                    }
                );
        }
    }
}
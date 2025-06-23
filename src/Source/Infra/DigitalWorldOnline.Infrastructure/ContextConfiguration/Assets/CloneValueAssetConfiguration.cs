using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class CloneValueAssetConfiguration : IEntityTypeConfiguration<CloneValueAssetDTO>
    {
        public void Configure(EntityTypeBuilder<CloneValueAssetDTO> builder)
        {
            builder
                .ToTable("CloneValue", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Type)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigicloneTypeEnum, int>(
                    x => (int)x,
                    x => (DigicloneTypeEnum)x))
                .IsRequired();

            builder
                .Property(x => x.MinLevel)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(x => x.MaxLevel)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(x => x.MinValue)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.MaxValue)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
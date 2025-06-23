using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class EvolutionAssetConfiguration : IEntityTypeConfiguration<EvolutionAssetDTO>
    {
        public void Configure(EntityTypeBuilder<EvolutionAssetDTO> builder)
        {
            builder
                .ToTable("Evolution", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.EvolutionRank)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<EvolutionRankEnum, int>(
                    x => (int)x,
                    x => (EvolutionRankEnum)x))
                .IsRequired();

            builder
                .HasMany(x => x.Lines)
                .WithOne(x => x.Evolution)
                .HasForeignKey(x => x.EvolutionId);
        }
    }
}
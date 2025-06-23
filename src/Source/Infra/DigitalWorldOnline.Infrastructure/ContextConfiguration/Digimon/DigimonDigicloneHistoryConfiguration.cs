using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Digimon
{
    public class DigimonDigicloneHistoryConfiguration : IEntityTypeConfiguration<DigimonDigicloneHistoryDTO>
    {
        public void Configure(EntityTypeBuilder<DigimonDigicloneHistoryDTO> builder)
        {
            builder
                .ToTable("DigicloneHistory", "Digimon")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ATValues)
                .HasConversion<IntArrayToStringConverter>();

            builder
                .Property(e => e.BLValues)
                .HasConversion<IntArrayToStringConverter>();

            builder
                .Property(e => e.CTValues)
                .HasConversion<IntArrayToStringConverter>();

            builder
                .Property(e => e.EVValues)
                .HasConversion<IntArrayToStringConverter>();

            builder
                .Property(e => e.HPValues)
                .HasConversion<IntArrayToStringConverter>();

        }
    }
}
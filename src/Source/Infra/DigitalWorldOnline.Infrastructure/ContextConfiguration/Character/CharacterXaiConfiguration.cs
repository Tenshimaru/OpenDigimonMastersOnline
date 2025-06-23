using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterXaiConfiguration : IEntityTypeConfiguration<CharacterXaiDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterXaiDTO> builder)
        {
            builder
                .ToTable("XaiInfo", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.ItemId)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
            
            builder
                .Property(x => x.XGauge)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();
            
            builder
                .Property(x => x.XCrystals)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();
        }
    }
}
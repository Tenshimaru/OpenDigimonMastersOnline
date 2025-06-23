using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterProgressConfiguration : IEntityTypeConfiguration<CharacterProgressDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterProgressDTO> builder)
        {
            builder
                .ToTable("Progress", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.CompletedData)
                .HasConversion<ByteArrayToStringConverter>();

            builder
                .Property(e => e.CompletedDataValue)
                .HasConversion<IntArrayToStringConverter>();

            builder
                .HasMany(x => x.InProgressQuestData)
                .WithOne(x => x.CharacterProgress)
                .HasForeignKey(x => x.CharacterProgressId);
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class QuestEventsAssetConfiguration : IEntityTypeConfiguration<QuestEventAssetDTO>
    {
        public void Configure(EntityTypeBuilder<QuestEventAssetDTO> builder)
        {
            builder
                .ToTable("QuestEvent", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.EventId)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
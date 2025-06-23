using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Models.Asset;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class AchievementAssetConfiguration : IEntityTypeConfiguration<AchievementAssetDTO>
    {
        public void Configure(EntityTypeBuilder<AchievementAssetDTO> builder)
        {
            builder
                .ToTable("Achievement", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.QuestId)
                .HasColumnType("int")
                .IsRequired();


            builder
                .Property(e => e.Type)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
               .Property(e => e.BuffId)
               .HasColumnType("int")
               .IsRequired();
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class QuestRewardObjectsAssetConfiguration : IEntityTypeConfiguration<QuestRewardObjectAssetDTO>
    {
        public void Configure(EntityTypeBuilder<QuestRewardObjectAssetDTO> builder)
        {
            builder
                .ToTable("QuestRewardObject", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Reward)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Amount)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
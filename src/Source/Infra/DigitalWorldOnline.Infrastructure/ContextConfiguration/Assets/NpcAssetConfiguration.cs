using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class NpcAssetConfiguration : IEntityTypeConfiguration<NpcAssetDTO>
    {
        public void Configure(EntityTypeBuilder<NpcAssetDTO> builder)
        {
            builder
                .ToTable("Npc", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.NpcId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.MapId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.Items)
                .WithOne(x => x.NpcAsset)
                .HasForeignKey(x => x.NpcAssetId);
        }
    }
}
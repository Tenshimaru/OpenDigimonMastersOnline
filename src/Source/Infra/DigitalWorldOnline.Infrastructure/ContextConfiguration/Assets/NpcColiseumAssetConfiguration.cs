using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class NpcColiseumAssetConfiguration : IEntityTypeConfiguration<NpcColiseumAssetDTO>
    {
        public void Configure(EntityTypeBuilder<NpcColiseumAssetDTO> builder)
        {
            builder
                .ToTable("NpcColiseum", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.NpcId)
                .HasColumnType("int")
                .IsRequired();


            builder
                .HasMany(x => x.MobInfo)
                .WithOne(x => x.NpcAsset)
                .HasForeignKey(x => x.NpcAssetId);
        }
    }
}
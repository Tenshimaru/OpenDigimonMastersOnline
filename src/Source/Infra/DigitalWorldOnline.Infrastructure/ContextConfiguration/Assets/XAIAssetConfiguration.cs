using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class XAIAssetConfiguration : IEntityTypeConfiguration<XaiAssetDTO>
    {
        public void Configure(EntityTypeBuilder<XaiAssetDTO> builder)
        {
            builder
                .ToTable("Xai", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.XGauge)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.XCrystals)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .HasData(//TODO: restante via tool
                    new XaiAssetDTO
                    {
                        Id = 1,
                        ItemId = 40017,
                        XGauge = 2000,
                        XCrystals = 3
                    });
        }
    }
}
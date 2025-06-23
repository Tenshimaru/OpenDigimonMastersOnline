using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class AccessoryRollStatusAssetConfiguration : IEntityTypeConfiguration<AccessoryRollStatusAssetDTO>
    {
        public void Configure(EntityTypeBuilder<AccessoryRollStatusAssetDTO> builder)
        {
            builder
                .ToTable("AccessoryRollStatus", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.MinValue)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.MaxValue)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
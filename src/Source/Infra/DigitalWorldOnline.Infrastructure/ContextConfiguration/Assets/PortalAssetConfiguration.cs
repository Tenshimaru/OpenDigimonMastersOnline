using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class PortalAssetConfiguration : IEntityTypeConfiguration<PortalAssetDTO>
    {
        public void Configure(EntityTypeBuilder<PortalAssetDTO> builder)
        {
            builder
                .ToTable("Portal", "Asset")
                .HasKey(x => x.Id);

            builder
            .Property(x => x.Type)
            .HasColumnType("int")
            .HasConversion(new ValueConverter<PortalTypeEnum, int>(
                x => (int)x,
                x => (PortalTypeEnum)x))
            .HasDefaultValue(PortalTypeEnum.Normal)
            .IsRequired();

            builder
                .Property(e => e.NpcId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.DestinationMapId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.DestinationX)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.DestinationY)
                .HasColumnType("int")
                .IsRequired();
            builder
               .Property(e => e.PortalIndex)
               .HasColumnType("int")
               .IsRequired();
            builder
               .Property(e => e.IsLocal)
               .HasColumnType("bit")
               .HasDefaultValue(0)
               .IsRequired();
        }
    }
}
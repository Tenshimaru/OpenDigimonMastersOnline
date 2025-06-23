using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class NpcPortalsAssetConfiguration : IEntityTypeConfiguration<NpcPortalsAssetDTO>
    {
        public void Configure(EntityTypeBuilder<NpcPortalsAssetDTO> builder)
        {
            builder
                .ToTable("NpcPortals", "Asset")
                .HasKey(x => x.Id);
            builder
            .Property(e => e.ItemId)
            .HasColumnType("int")
            .IsRequired();

            builder
                .Property(e => e.Type)
            .HasColumnType("int")
                .HasConversion(new ValueConverter<NpcResourceTypeEnum, int>(
                    x => (int)x,
                    x => (NpcResourceTypeEnum)x))
                .HasDefaultValue(NpcResourceTypeEnum.None)
                .IsRequired();

            builder
               .Property(e => e.ResourceAmount)
               .HasColumnType("int")
               .IsRequired();


        }
    }
}
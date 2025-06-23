using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class NpcMobInfoAssetConfiguration : IEntityTypeConfiguration<NpcMobInfoAssetDTO>
    {
        public void Configure(EntityTypeBuilder<NpcMobInfoAssetDTO> builder)
        {
            builder
                .ToTable("NpcMobInfo", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.SummonType)
                .HasColumnType("int")
                .IsRequired();

            builder
             .Property(e => e.Round)
             .HasColumnType("int")
             .IsRequired();

            builder
               .Property(e => e.WinPoints)
               .HasColumnType("int")
               .IsRequired();

            builder
              .Property(e => e.WinPoints)
              .HasColumnType("int")
              .IsRequired();
        }
    }
}
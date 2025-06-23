using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class HatchAssetConfiguration : IEntityTypeConfiguration<HatchAssetDTO>
    {
        public void Configure(EntityTypeBuilder<HatchAssetDTO> builder)
        {
            builder
                .ToTable("Hatch", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.HatchType)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.LowClassDataSection)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.MidClassDataSection)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.LowClassDataAmount)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.MidClassDataAmount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.LowClassLimitLevel)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.MidClassLimitLevel)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.LowClassBreakPoint)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.MidClassBreakPoint)
                .HasColumnType("int")
                .IsRequired();

        }
    }
}
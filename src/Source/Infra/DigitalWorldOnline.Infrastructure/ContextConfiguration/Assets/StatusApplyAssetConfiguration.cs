using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class StatusApplyAssetConfiguration : IEntityTypeConfiguration<StatusApplyAssetDTO>
    {
        public void Configure(EntityTypeBuilder<StatusApplyAssetDTO> builder)
        {
            builder
                .ToTable("StatusApply", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.StageValue)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.ApplyValue)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
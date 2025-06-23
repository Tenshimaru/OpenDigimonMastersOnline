using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class CloneAssetConfiguration : IEntityTypeConfiguration<CloneAssetDTO>
    {
        public void Configure(EntityTypeBuilder<CloneAssetDTO> builder)
        {
            builder
                .ToTable("Clone", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.ItemSection)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.MinLevel)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(x => x.MaxLevel)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(x => x.Bits)
                .HasColumnType("bigint")
                .IsRequired();
        }
    }
}
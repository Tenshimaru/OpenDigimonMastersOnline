using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ExtraEvolutionRequiredAssetConfiguration : IEntityTypeConfiguration<ExtraEvolutionRequiredAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ExtraEvolutionRequiredAssetDTO> builder)
        {
            builder
           .ToTable("ExtraEvolutionRequired", "Asset")
           .HasKey(x => x.Id);

            builder
            .Property(e => e.ItemId)
            .HasColumnType("int")
            .IsRequired();

            builder
           .Property(e => e.Amount)
           .HasColumnType("int")
           .IsRequired();

        }
    }
}

using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ExtraEvolutionMaterialAssetConfiguration : IEntityTypeConfiguration<ExtraEvolutionMaterialAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ExtraEvolutionMaterialAssetDTO> builder)
        {
            builder
           .ToTable("ExtraEvolutionMaterial", "Asset")
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

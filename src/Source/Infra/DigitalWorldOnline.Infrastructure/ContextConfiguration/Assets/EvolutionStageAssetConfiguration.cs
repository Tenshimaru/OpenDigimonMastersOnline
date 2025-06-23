using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class EvolutionStageAssetConfiguration : IEntityTypeConfiguration<EvolutionStageAssetDTO>
    {
        public void Configure(EntityTypeBuilder<EvolutionStageAssetDTO> builder)
        {
            builder
                .ToTable("EvolutionStage", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Value)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
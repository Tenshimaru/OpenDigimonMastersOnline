using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ExtraEvolutionInformationAssetConfiguration : IEntityTypeConfiguration<ExtraEvolutionInformationAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ExtraEvolutionInformationAssetDTO> builder)
        {
            builder
           .ToTable("ExtraEvolutionInformation", "Asset")
           .HasKey(x => x.Id);

            builder
            .Property(e => e.IndexId)
            .HasColumnType("int")
            .IsRequired();

            builder
              .HasMany(x => x.ExtraEvolution)
              .WithOne(x => x.ExtraInformationAsset)
              .HasForeignKey(x => x.ExtraInfoId);
        }
    }
}

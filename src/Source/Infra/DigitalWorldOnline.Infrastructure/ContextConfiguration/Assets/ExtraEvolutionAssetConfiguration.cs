using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ExtraEvolutionAssetConfiguration : IEntityTypeConfiguration<ExtraEvolutionAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ExtraEvolutionAssetDTO> builder)
        {
            builder
           .ToTable("ExtraEvolution", "Asset")
           .HasKey(x => x.Id);

            builder
               .Property(e => e.DigimonId)
               .HasColumnType("int")
               .IsRequired();

            builder
              .Property(e => e.Price)
              .HasColumnType("bigint")
              .IsRequired();


            builder
              .Property(e => e.RequiredLevel)
              .HasColumnType("tinyint")
              .IsRequired();

            builder
               .HasMany(x => x.Materials)
               .WithOne(x => x.ExtraEvolution)
               .HasForeignKey(x => x.ExtraEvolutionId);

            builder
             .HasMany(x => x.Requireds)
             .WithOne(x => x.ExtraEvolution)
             .HasForeignKey(x => x.ExtraEvolutionId);
        }
    }
}

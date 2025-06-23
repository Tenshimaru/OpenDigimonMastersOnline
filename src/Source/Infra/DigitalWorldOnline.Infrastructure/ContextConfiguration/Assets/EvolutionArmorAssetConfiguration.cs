using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class EvolutionArmorAssetConfiguration : IEntityTypeConfiguration<EvolutionArmorAssetDTO>
    {
        public void Configure(EntityTypeBuilder<EvolutionArmorAssetDTO> builder)
        {
            builder
                .ToTable("EvolutionArmor", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                 .Property(e => e.Amount)
                 .HasColumnType("int")
                 .IsRequired();

            builder
                .Property(e => e.Chance)
                .HasColumnType("int")
                .IsRequired();

        }
    }
}
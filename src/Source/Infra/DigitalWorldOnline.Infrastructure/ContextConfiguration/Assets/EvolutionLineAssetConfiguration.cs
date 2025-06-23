using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class EvolutionLineAssetConfiguration : IEntityTypeConfiguration<EvolutionLineAssetDTO>
    {
        public void Configure(EntityTypeBuilder<EvolutionLineAssetDTO> builder)
        {
            builder
                .ToTable("EvolutionLine", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.SlotLevel)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.UnlockLevel)
                .HasColumnType("tinyint")
                .IsRequired();
            
            builder
                .Property(e => e.UnlockQuestId)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(e => e.UnlockItemSection)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.UnlockItemSectionAmount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RequiredItem)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RequiredAmount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.Stages)
                .WithOne(x => x.EvolutionLine)
                .HasForeignKey(x => x.EvolutionLineId);
        }
    }
}
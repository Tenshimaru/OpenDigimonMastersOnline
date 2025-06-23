using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class ContainerRewardAssetConfiguration : IEntityTypeConfiguration<ContainerRewardAssetDTO>
    {
        public void Configure(EntityTypeBuilder<ContainerRewardAssetDTO> builder)
        {
            builder
                .ToTable("ContainerReward", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.ItemName)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            
            builder
                .Property(e => e.MinAmount)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
            
            builder
                .Property(e => e.MaxAmount)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Chance)
                .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();


            builder
                .Property(e => e.Rare)
                .HasColumnType("bit")
                .IsRequired();
        }
    }
}
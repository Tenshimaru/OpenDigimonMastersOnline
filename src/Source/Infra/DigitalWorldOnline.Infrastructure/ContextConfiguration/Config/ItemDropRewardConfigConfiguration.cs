using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class ItemDropRewardConfigConfiguration : IEntityTypeConfiguration<ItemDropConfigDTO>
    {
        public void Configure(EntityTypeBuilder<ItemDropConfigDTO> builder)
        {
            builder
                .ToTable("ItemDropReward", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
            
            builder
                .Property(e => e.MinAmount)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.MaxAmount)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
            
            builder
                .Property(e => e.Chance)
               .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.Rank)
                .HasColumnType("int")
                .HasDefaultValue(1)
                .IsRequired();
        }
    }
}
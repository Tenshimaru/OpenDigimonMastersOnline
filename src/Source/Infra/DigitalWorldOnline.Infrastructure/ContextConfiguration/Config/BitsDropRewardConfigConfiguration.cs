using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class BitsDropRewardConfigConfiguration : IEntityTypeConfiguration<BitsDropConfigDTO>
    {
        public void Configure(EntityTypeBuilder<BitsDropConfigDTO> builder)
        {
            builder
                .ToTable("BitsDropReward", "Config")
                .HasKey(x => x.Id);
            
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

        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class FruitConfiguration : IEntityTypeConfiguration<FruitConfigDTO>
    {
        public void Configure(EntityTypeBuilder<FruitConfigDTO> builder)
        {
            builder
                .ToTable("Fruit", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.ItemSection)
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasMany(x => x.SizeList)
                .WithOne(x => x.FruitConfig)
                .HasForeignKey(x => x.FruitConfigId);
        }
    }
}
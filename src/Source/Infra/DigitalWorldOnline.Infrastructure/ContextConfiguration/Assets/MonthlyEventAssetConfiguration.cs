using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class MonthlyAssetConfiguration : IEntityTypeConfiguration<MonthlyEventAssetDTO>
    {
        public void Configure(EntityTypeBuilder<MonthlyEventAssetDTO> builder)
        {
            builder
                .ToTable("MonthlyEvent", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.CurrentDay)
                .HasColumnType("int")
                .IsRequired();


            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
               .Property(e => e.ItemCount)
               .HasColumnType("int")
               .IsRequired();
        }
    }
}
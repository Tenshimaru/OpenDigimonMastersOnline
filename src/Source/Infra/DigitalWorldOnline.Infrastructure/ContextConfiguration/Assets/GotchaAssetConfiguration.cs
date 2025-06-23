using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class GotchaAssetConfiguration : IEntityTypeConfiguration<GotchaAssetDTO>
    {
        public void Configure(EntityTypeBuilder<GotchaAssetDTO> builder)
        {

            builder
                    .ToTable("Gotcha", "Asset")
                    .HasKey(x => x.Id);

            builder
                .Property(e => e.Id)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.GotchaId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.NpcId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.UseItem)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.UseCount)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Limit)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(e => e.MinLv)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(e => e.MaxLv)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(e => e.RareItemCnt)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(e => e.Chance)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .HasMany(x => x.Items)
                .WithOne(x => x.Gotcha)
                .HasForeignKey(x => x.GotchaId);

            builder
                .HasMany(x => x.RareItems)
                .WithOne(x => x.Gotcha)
                .HasForeignKey(x => x.GotchaId);
        }
    }
}
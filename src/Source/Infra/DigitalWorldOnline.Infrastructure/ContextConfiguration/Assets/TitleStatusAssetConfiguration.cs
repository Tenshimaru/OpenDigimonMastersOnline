using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class TitleStatusAssetConfiguration : IEntityTypeConfiguration<TitleStatusAssetDTO>
    {
        public void Configure(EntityTypeBuilder<TitleStatusAssetDTO> builder)
        {
            builder
                .ToTable("TitleStatus", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.ItemId)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(e => e.AchievementId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.ARValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.ASValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.ATValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.BLValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.CTValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.DEValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.DSValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.EVValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.HPValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.HTValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.MSValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.WSValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
            
            builder
                .Property(e => e.SCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.LASCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.FISCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.ICSCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.LISCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.STSCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.NESCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.DASCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.THSCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.WASCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.WISCD)
                .HasColumnType("numeric")
                .IsRequired();
            
            builder
                .Property(e => e.WOSCD)
                .HasColumnType("numeric")
                .IsRequired();
        }
    }
}
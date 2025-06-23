using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Mechanics
{
    public class GuildHistoricConfiguration : IEntityTypeConfiguration<GuildHistoricDTO>
    {
        public void Configure(EntityTypeBuilder<GuildHistoricDTO> builder)
        {
            builder
                .ToTable("Historic", "Guild")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Type)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<GuildHistoricTypeEnum, int>(
                    x => (int)x,
                    x => (GuildHistoricTypeEnum)x))
                .IsRequired();

            builder
                .Property(x => x.Date)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .Property(x => x.MasterClass)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<GuildAuthorityTypeEnum, int>(
                    x => (int)x,
                    x => (GuildAuthorityTypeEnum)x))
                .IsRequired();

            builder
                .Property(x => x.MasterName)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(x => x.MemberClass)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<GuildAuthorityTypeEnum, int>(
                    x => (int)x,
                    x => (GuildAuthorityTypeEnum)x))
                .IsRequired();

            builder
                .Property(x => x.MemberName)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Mechanics
{
    public class GuildAuthorityConfiguration : IEntityTypeConfiguration<GuildAuthorityDTO>
    {
        public void Configure(EntityTypeBuilder<GuildAuthorityDTO> builder)
        {
            builder
                .ToTable("Authority", "Guild")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Class)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<GuildAuthorityTypeEnum, int>(
                    x => (int)x,
                    x => (GuildAuthorityTypeEnum)x))
                .HasDefaultValue(GuildAuthorityTypeEnum.Member)
                .IsRequired();

            builder
                .Property(x => x.Duty)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();
            
            builder
                .Property(x => x.Title)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
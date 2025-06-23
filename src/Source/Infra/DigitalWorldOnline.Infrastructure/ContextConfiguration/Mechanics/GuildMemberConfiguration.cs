using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Mechanics
{
    public class GuildMemberConfiguration : IEntityTypeConfiguration<GuildMemberDTO>
    {
        public void Configure(EntityTypeBuilder<GuildMemberDTO> builder)
        {
            builder
                .ToTable("Member", "Guild")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Contribution)
                .HasColumnType("int")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .Property(x => x.Authority)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<GuildAuthorityTypeEnum, int>(
                    x => (int)x,
                    x => (GuildAuthorityTypeEnum)x))
                .HasDefaultValue(GuildAuthorityTypeEnum.Member)
                .IsRequired();

            builder
                .HasOne(x => x.Character);
        }
    }
}
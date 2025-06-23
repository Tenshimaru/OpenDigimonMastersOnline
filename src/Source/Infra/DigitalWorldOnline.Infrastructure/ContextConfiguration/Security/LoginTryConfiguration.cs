using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.DTOs.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Security
{
    public class LoginTryConfiguration : IEntityTypeConfiguration<LoginTryDTO>
    {
        public void Configure(EntityTypeBuilder<LoginTryDTO> builder)
        {
            builder
                .ToTable("LoginTry", "Security")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Username)
                .HasColumnType("varchar")
                .HasMaxLength(20)
                .IsRequired();

            builder
                .Property(x => x.Date)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .Property(x => x.Ip)
                .HasColumnType("varchar")
                .HasMaxLength(30)
                .IsRequired();

            builder
                .Property(x => x.Result)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<LoginTryResultEnum, int>(
                    x => (int)x,
                    x => (LoginTryResultEnum)x))
                .HasDefaultValue(LoginTryResultEnum.Success)
                .IsRequired();
        }
    }
}
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.DTOs.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Account
{
    public class AccountBlockConfiguration : IEntityTypeConfiguration<AccountBlockDTO>
    {
        public void Configure(EntityTypeBuilder<AccountBlockDTO> builder)
        {
            builder
                .ToTable("AccountBlock", "Account")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Type)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<AccountBlockEnum, int>(
                    x => (int)x,
                    x => (AccountBlockEnum)x))
                .HasDefaultValue(AccountBlockEnum.Unblocked)
                .IsRequired();

            builder
                .Property(x => x.Reason)
                .HasColumnType("varchar")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property(x => x.StartDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .Property(x => x.EndDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();
        }
    }
}
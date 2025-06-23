using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.DTOs.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Account
{
    public class AccountConfiguration : IEntityTypeConfiguration<AccountDTO>
    {
        public void Configure(EntityTypeBuilder<AccountDTO> builder)
        {
            builder
                .ToTable("Account", "Account")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Username)
                .HasColumnType("varchar")
                .HasMaxLength(20)
                .IsRequired();

            builder
                .Property(x => x.Password)
                .HasColumnType("varchar")
                .HasMaxLength(250)
                .IsRequired();

            builder
                .Property(x => x.SecondaryPassword)
                .HasColumnType("varchar")
                .HasMaxLength(250);

            builder
                .Property(x => x.Email)
                .HasColumnType("varchar")
                .HasMaxLength(150)
                .IsRequired();
            
            builder
                .Property(x => x.DiscordId)
                .HasColumnType("varchar")
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(x => x.AccessLevel)
                .HasConversion(new ValueConverter<AccountAccessLevelEnum, int>(
                    x => (int)x,
                    x => (AccountAccessLevelEnum)x))
                .HasColumnType("int")
                .HasDefaultValue(AccountAccessLevelEnum.Default)
                .IsRequired();

            builder
                .Property(x => x.CreateDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .Property(x => x.LastConnection)
                .HasColumnType("datetime2");

            builder
                .Property(x => x.MembershipExpirationDate)
                .HasColumnType("datetime2");

            builder
                .Property(x => x.Premium)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.Silk)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.LastPlayedServer)
                .HasColumnType("bigint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.LastPlayedCharacter)
                .HasColumnType("bigint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.ReceiveWelcome)
                .HasColumnType("bit")
                .IsRequired();

            builder
                .HasMany(c => c.ItemList)
                .WithOne()
                .HasForeignKey("AccountId")
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.SystemInformation)
                .WithOne(x => x.Account)
                .HasForeignKey<SystemInformationDTO>(x => x.AccountId);

            builder
                .HasOne(x => x.AccountBlock)
                .WithOne(x => x.Account)
                .HasForeignKey<AccountBlockDTO>(x => x.AccountId);
        }
    }
}

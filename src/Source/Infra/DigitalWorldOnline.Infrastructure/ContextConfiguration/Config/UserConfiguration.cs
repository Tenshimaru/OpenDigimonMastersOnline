using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class UserConfiguration : IEntityTypeConfiguration<UserDTO>
    {
        public void Configure(EntityTypeBuilder<UserDTO> builder)
        {
            builder
                .ToTable("User", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Username)
                .HasColumnType("varchar")
                .HasMaxLength(25)
                .IsRequired();

            builder
                .Property(x => x.Password)
                .HasColumnType("varchar")
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(x => x.AccessLevel)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<UserAccessLevelEnum, int>(
                    x => (int)x,
                    x => (UserAccessLevelEnum)x))
                .HasDefaultValue(UserAccessLevelEnum.Default)
                .IsRequired();

            builder.HasData(new UserDTO()
            {
                Id = 1,
                Username = "masteradmin",
                Password = "pMgM+NOH0Z+RwR9F1iFVOOwKrW1iDaifx4jWDnH1Dbo=",
                AccessLevel = UserAccessLevelEnum.Administrator
            });
        }
    }
}
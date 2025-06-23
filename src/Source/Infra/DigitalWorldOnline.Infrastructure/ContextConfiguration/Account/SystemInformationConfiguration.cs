using DigitalWorldOnline.Commons.DTOs.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Account
{
    public class SystemInformationConfiguration : IEntityTypeConfiguration<SystemInformationDTO>
    {
        public void Configure(EntityTypeBuilder<SystemInformationDTO> builder)
        {
            builder
                .ToTable("SystemInformation", "Account")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Cpu)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder
                .Property(x => x.Gpu)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder
                .Property(x => x.Ip)
                .HasColumnType("varchar")
                .HasMaxLength(30);
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class WelcomeMessageConfigConfiguration : IEntityTypeConfiguration<WelcomeMessageConfigDTO>
    {
        public void Configure(EntityTypeBuilder<WelcomeMessageConfigDTO> builder)
        {
            builder
                .ToTable("WelcomeMessage", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Message)
                .HasColumnType("varchar")
                .HasMaxLength(150)
                .IsRequired();

            builder
                .Property(e => e.Enabled)
                .HasColumnType("bit")
                .IsRequired();

            builder
                .HasData(
                    new WelcomeMessageConfigDTO()
                    {
                        Id = 1,
                        Message = "Welcome to UDMO - Ultra Digital Masters online",
                        Enabled = true
                    }
                );
        }
    }
}
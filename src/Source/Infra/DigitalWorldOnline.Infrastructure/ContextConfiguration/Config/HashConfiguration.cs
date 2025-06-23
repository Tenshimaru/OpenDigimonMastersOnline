using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config
{
    public class HashConfiguration : IEntityTypeConfiguration<HashDTO>
    {
        public void Configure(EntityTypeBuilder<HashDTO> builder)
        {
            builder
                .ToTable("Hash", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Hash)
                .HasColumnType("varchar")
                .HasMaxLength(1000)
                .IsRequired();

            builder
                .Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.HasData(new HashDTO()
            {
                Id = 1,
                Hash = "pMgM+NOH0Z+RwR9F1iFVOOwKrW1iDaifx4jWDnH1Dbo=",
                CreatedAt = DateTime.Now
            });
        }
    }
}
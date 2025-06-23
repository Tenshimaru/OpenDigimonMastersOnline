
using DigitalWorldOnline.Commons.DTOs.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Arena
{
    public class CharacterArenaDailyPointsConfiguration : IEntityTypeConfiguration<CharacterArenaDailyPointsDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterArenaDailyPointsDTO> builder)
        {
            builder
                .ToTable("DailyPoints", "Character")
                .HasKey(x => x.Id);


            builder
             .Property(x => x.InsertDate)
             .HasColumnType("datetime2")
             .HasDefaultValueSql("getdate()")
             .IsRequired();

            builder
              .Property(x => x.Points)
              .HasColumnType("int")
              .HasDefaultValueSql("0")
              .IsRequired();

        }
    }
}
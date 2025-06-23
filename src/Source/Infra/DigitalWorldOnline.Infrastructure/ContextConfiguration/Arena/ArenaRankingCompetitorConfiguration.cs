
using DigitalWorldOnline.Commons.DTOs.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Arena
{
    public class ArenaRankingCompetitorConfiguration : IEntityTypeConfiguration<ArenaRankingCompetitorDTO>
    {
        public void Configure(EntityTypeBuilder<ArenaRankingCompetitorDTO> builder)
        {
            builder
                .ToTable("Competitor", "Arena")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.TamerId)
                .HasColumnType("long")
                .HasDefaultValueSql("0")
                .IsRequired();

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

            builder
              .Property(x => x.Position)
              .HasColumnType("tinyint")
              .HasDefaultValueSql("0")
              .IsRequired();

            builder
             .Property(x => x.New)
             .HasColumnType("bit")
             .HasDefaultValueSql("0")
             .IsRequired();
     

        }
    }
}
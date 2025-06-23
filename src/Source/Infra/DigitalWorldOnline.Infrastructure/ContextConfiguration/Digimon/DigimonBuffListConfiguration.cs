using DigitalWorldOnline.Commons.DTOs.Digimon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Digimon
{
    public class DigimonBuffListConfiguration : IEntityTypeConfiguration<DigimonBuffListDTO>
    {
        public void Configure(EntityTypeBuilder<DigimonBuffListDTO> builder)
        {
            builder
                .ToTable("BuffList", "Digimon")
                .HasKey(x => x.Id);

            builder
                .HasMany(x => x.Buffs)
                .WithOne(x => x.BuffList)
                .HasForeignKey(x => x.BuffListId);
        }
    }
}
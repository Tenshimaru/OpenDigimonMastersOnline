using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterArenaPointsConfiguration : IEntityTypeConfiguration<CharacterArenaPointsDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterArenaPointsDTO> builder)
        {
            builder
                .ToTable("ArenaPoints", "Character")
                .HasKey(x => x.Id);
            
            builder
                .Property(x => x.ItemId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.Amount)
                .HasColumnType("int")
                .IsRequired();

            builder
               .Property(x => x.CurrentStage)
               .HasColumnType("int")
               .IsRequired();
         
        }
    }
}
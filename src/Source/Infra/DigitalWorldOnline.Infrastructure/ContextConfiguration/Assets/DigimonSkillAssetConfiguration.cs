using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class DigimonSkillAssetConfiguration : IEntityTypeConfiguration<DigimonSkillAssetDTO>
    {
        public void Configure(EntityTypeBuilder<DigimonSkillAssetDTO> builder)
        {
            builder
                .ToTable("DigimonSkill", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Slot)
                .HasColumnType("tinyint")
                .IsRequired();
            
            builder
                .Property(e => e.SkillId)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
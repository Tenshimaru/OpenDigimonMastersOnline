using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class MonsterSkillAssetConfiguration : IEntityTypeConfiguration<MonsterSkillAssetDTO>
    {
        public void Configure(EntityTypeBuilder<MonsterSkillAssetDTO> builder)
        {
            builder
                .ToTable("MonsterSkill", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();

            
            builder
                .Property(e => e.SkillId)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
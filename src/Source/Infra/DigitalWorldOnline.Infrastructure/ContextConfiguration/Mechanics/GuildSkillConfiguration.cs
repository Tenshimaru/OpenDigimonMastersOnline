using DigitalWorldOnline.Commons.DTOs.Mechanics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Mechanics
{
    public class GuildSkillConfiguration : IEntityTypeConfiguration<GuildSkillDTO>
    {
        public void Configure(EntityTypeBuilder<GuildSkillDTO> builder)
        {
            builder
                .ToTable("Skill", "Guild")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Level)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();
            
            builder
                .Property(x => x.SkillId)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
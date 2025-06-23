using DigitalWorldOnline.Commons.DTOs.Config.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.Events
{
    public class EventMobExpRewardConfigConfiguration : IEntityTypeConfiguration<EventMobExpRewardConfigDTO>
    {
        public void Configure(EntityTypeBuilder<EventMobExpRewardConfigDTO> builder)
        {
            builder
                .ToTable("EventMobExpReward", "Config")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.TamerExperience)
                .HasColumnType("bigint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.DigimonExperience)
                .HasColumnType("bigint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.NatureExperience)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.ElementExperience)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.SkillExperience)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();
        }
    }
}
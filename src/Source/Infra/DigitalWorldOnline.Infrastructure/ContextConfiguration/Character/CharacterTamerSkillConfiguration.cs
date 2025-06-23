
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterTamerSkillConfiguration : IEntityTypeConfiguration<CharacterTamerSkillDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterTamerSkillDTO> builder)
        {
            builder
                .ToTable("ActiveSkill", "Character")
                .HasKey(x => x.Id);

            builder
              .Property(x => x.Type)
              .HasColumnType("int")
              .HasConversion(new ValueConverter<TamerSkillTypeEnum, int>(
                  x => (int)x,
                  x => (TamerSkillTypeEnum)x))
              .HasDefaultValue(TamerSkillTypeEnum.Normal)
              .IsRequired();

                builder
              .Property(x => x.Duration)
              .HasColumnType("int")
              .HasDefaultValueSql("0")
              .IsRequired();
             
                builder
                 .Property(x => x.EndDate)
                 .HasColumnType("datetime2")
                 .HasDefaultValueSql("getdate()")
                 .IsRequired();

            builder
            .Property(x => x.Cooldown)
            .HasColumnType("int")
            .HasDefaultValueSql("0")
            .IsRequired();

            builder
             .Property(x => x.EndCooldown)
             .HasColumnType("datetime2")
             .HasDefaultValueSql("getdate()")
             .IsRequired();

        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.Events
{
    public class EventMobConfigConfiguration : IEntityTypeConfiguration<EventMobConfigDTO>
    {
        public void Configure(EntityTypeBuilder<EventMobConfigDTO> builder)
        {
            builder
                .ToTable("EventMob", "Config")
                .HasKey(x => x.Id);

            // Configure properties
            builder.Property(e => e.EventMapConfigId)
                .IsRequired();

            builder
                .Property(e => e.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(e => e.Level)
                .HasColumnType("tinyint")
                .HasDefaultValue(1)
                .IsRequired();

            builder
                .Property(e => e.Model)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RespawnInterval)
                .HasColumnType("int")
                .HasDefaultValue(5)
                .IsRequired();

            builder
                .Property(e => e.Duration)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.Class)
                .HasColumnType("int")
                .IsRequired();
            

            builder
                .Property(e => e.Round)
                .HasColumnType("tinyint")
                .HasDefaultValue(1)
                .IsRequired();

            builder
                .Property(e => e.ARValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.ASValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.ATValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.BLValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.CTValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.DEValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.DSValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.EVValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.HPValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.HTValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.MSValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.WSValue)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.ViewRange)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(e => e.HuntRange)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();


            builder
                .Property(x => x.ReactionType)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonReactionTypeEnum, int>(
                    x => (int)x,
                    x => (DigimonReactionTypeEnum)x))
                .HasDefaultValue(DigimonReactionTypeEnum.Passive)
                .IsRequired();

            builder
                .Property(e => e.DeathTime)
                .HasColumnType("datetime")
                .HasDefaultValue(null);

            builder
                .Property(e => e.ResurrectionTime)
                .HasColumnType("datetime")
                .HasDefaultValue(null);

            builder
                .Property(x => x.Attribute)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonAttributeEnum, int>(
                    x => (int)x,
                    x => (DigimonAttributeEnum)x))
                .HasDefaultValue(DigimonAttributeEnum.None)
                .IsRequired();

            builder
                .Property(x => x.Element)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonElementEnum, int>(
                    x => (int)x,
                    x => (DigimonElementEnum)x))
                .HasDefaultValue(DigimonElementEnum.Neutral)
                .IsRequired();

            builder
                .Property(x => x.Family1)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonFamilyEnum, int>(
                    x => (int)x,
                    x => (DigimonFamilyEnum)x))
                .HasDefaultValue(DigimonFamilyEnum.None)
                .IsRequired();

            builder
                .Property(x => x.Family2)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonFamilyEnum, int>(
                    x => (int)x,
                    x => (DigimonFamilyEnum)x))
                .HasDefaultValue(DigimonFamilyEnum.None)
                .IsRequired();

            builder
                .Property(x => x.Family3)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<DigimonFamilyEnum, int>(
                    x => (int)x,
                    x => (DigimonFamilyEnum)x))
                .HasDefaultValue(DigimonFamilyEnum.None)
                .IsRequired();

            builder
                .HasOne(x => x.Location)
                .WithOne(x => x.MobConfig)
                .HasForeignKey<EventMobLocationConfigDTO>(x => x.MobConfigId);

            builder
                .HasOne(x => x.DropReward)
                .WithOne(x => x.Mob)
                .HasForeignKey<EventMobDropRewardConfigDTO>(x => x.MobId);

            builder
                .HasOne(x => x.ExpReward)
                .WithOne(x => x.Mob)
                .HasForeignKey<EventMobExpRewardConfigDTO>(x => x.MobId);

            builder
                .HasOne(x => x.EventMapConfig)
                .WithMany(x => x.Mobs)
                .HasForeignKey(x => x.EventMapConfigId);
        }
    }
}
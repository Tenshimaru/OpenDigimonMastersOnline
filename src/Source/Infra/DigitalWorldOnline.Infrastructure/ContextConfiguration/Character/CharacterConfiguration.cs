using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.DTOs.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterConfiguration : IEntityTypeConfiguration<CharacterDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterDTO> builder)
        {
            builder
                .ToTable("Tamer", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.AccountId)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(x => x.Position)
                .HasColumnType("smallint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .Property(x => x.CreateDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .Property(x => x.Model)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<CharacterModelEnum, int>(
                    x => (int)x,
                    x => (CharacterModelEnum)x))
                .HasDefaultValue(CharacterModelEnum.MarcusDamon)
                .IsRequired();

            builder
                .Property(x => x.Level)
                .HasColumnType("tinyint")
                .HasDefaultValue(1)
                .IsRequired();

            builder
                .Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(25)
                .IsRequired();

            builder
                .Property(x => x.Size)
                .HasColumnType("smallint")
                .HasDefaultValue(10000)
                .IsRequired();

            builder
                .Property(x => x.State)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<CharacterStateEnum, int>(
                    x => (int)x,
                    x => (CharacterStateEnum)x))
                .HasDefaultValue(CharacterStateEnum.Disconnected)
                .IsRequired();

            builder
                .Property(x => x.EventState)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<CharacterEventStateEnum, int>(
                    x => (int)x,
                    x => (CharacterEventStateEnum)x))
                .HasDefaultValue(CharacterEventStateEnum.None)
                .IsRequired();

            builder
                .Property(x => x.ServerId)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(x => x.CurrentExperience)
                .HasColumnType("bigint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.Channel)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MaxValue)
                .IsRequired();

            builder
                .Property(x => x.TargetTamerIdTP)
                .HasColumnType("int")
                .HasDefaultValue(byte.MaxValue)
                .IsRequired();

            builder
                .Property(x => x.DigimonSlots)
                .HasColumnType("tinyint")
                .HasDefaultValue(3)
                .IsRequired();

            builder
                .Property(x => x.CurrentHp)
                .HasColumnType("int")
                .HasDefaultValue(50)
                .IsRequired();

            builder
                .Property(x => x.CurrentDs)
                .HasColumnType("int")
                .HasDefaultValue(40)
                .IsRequired();

            builder
                .Property(x => x.XGauge)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.XCrystals)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.CurrentTitle)
                .HasColumnType("smallint")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.DeckBuffId)
                .HasColumnType("int");

            builder
                .Property(x => x.InitialPacketSentOnceSent)
                .HasColumnType("bit")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .HasMany(c => c.ItemList)
                .WithOne()
                .HasForeignKey("CharacterId")
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.BuffList)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterBuffListDTO>(x => x.CharacterId);


            builder
                .HasOne(x => x.Location)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterLocationDTO>(x => x.CharacterId);

            builder
                .HasMany(x => x.Digimons)
                .WithOne(x => x.Character)
                .HasForeignKey(x => x.CharacterId);

            builder
                .HasMany(x => x.Friends)
                .WithOne(x => x.Character)
                .HasForeignKey(x => x.CharacterId);

            builder
                .HasMany(x => x.Friended)
                .WithOne(x => x.Friend)
                .HasForeignKey(x => x.FriendId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(x => x.Foes)
                .WithOne(x => x.Character)
                .HasForeignKey(x => x.CharacterId);

            builder
                .HasMany(x => x.Foed)
                .WithOne(x => x.Foe)
                .HasForeignKey(x => x.FoeId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(x => x.Points)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterArenaPointsDTO>(x => x.CharacterId);

            builder
                .HasOne(x => x.Incubator)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterIncubatorDTO>(x => x.CharacterId);

            builder
                .HasMany(x => x.MapRegions)
                .WithOne(x => x.Character)
                .HasForeignKey(x => x.CharacterId);

            builder
                .HasOne(x => x.SealList)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterSealListDTO>(x => x.CharacterId);

            builder
                .HasOne(x => x.Xai)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterXaiDTO>(x => x.CharacterId);

            builder
                .HasOne(x => x.TimeReward)
                .WithOne(x => x.Character)
                .HasForeignKey<TimeRewardDTO>(x => x.CharacterId);

            builder
                .HasOne(x => x.AttendanceReward)
                .WithOne(x => x.Character)
                .HasForeignKey<AttendanceRewardDTO>(x => x.CharacterId);

            builder
                .HasOne(x => x.DailyPoints)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterArenaDailyPointsDTO>(x => x.CharacterId);

            builder
                .HasOne(x => x.ConsignedShop)
                .WithOne(x => x.Character)
                .HasForeignKey<ConsignedShopDTO>(x => x.CharacterId);

            builder
                .HasOne(x => x.Progress)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterProgressDTO>(x => x.CharacterId);

            builder
                .HasOne(x => x.ActiveEvolution)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterActiveEvolutionDTO>(x => x.CharacterId);

            builder
                .HasOne(x => x.DigimonArchive)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterDigimonArchiveDTO>(x => x.CharacterId);

            builder
                .HasMany(x => x.ActiveSkill)
                .WithOne(x => x.Character)
                .HasForeignKey(x => x.CharacterId);

            builder
                .HasOne(x => x.DeckBuff)
                .WithMany(x => x.Characters)
                .HasForeignKey(x => x.DeckBuffId)
                .HasPrincipalKey(x => x.GroupIdX);
        }
    }
}
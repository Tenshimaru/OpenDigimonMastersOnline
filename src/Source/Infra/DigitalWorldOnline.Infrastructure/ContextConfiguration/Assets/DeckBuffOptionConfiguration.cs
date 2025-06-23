using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class DeckBuffOptionConfiguration : IEntityTypeConfiguration<DeckBuffOptionAssetDTO>
    {
        public void Configure(EntityTypeBuilder<DeckBuffOptionAssetDTO> builder)
        {
            builder
                .ToTable("DeckBuffOption", "Asset")
                .HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder
                .Property(e => e.GroupIdX)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Condition)
                .HasConversion(new ValueConverter<DeckBuffConditionsEnum, int>(
                    x => (int)x,
                    x => (DeckBuffConditionsEnum)x))
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.AtType)
                .HasConversion(new ValueConverter<DeckBuffAtTypesEnum, int>(
                    x => (int)x,
                    x => (DeckBuffAtTypesEnum)x))
                .HasColumnType("int")
                .HasDefaultValue(DeckBuffAtTypesEnum.Passive);

            builder
                .Property(e => e.OptionId)
                .HasColumnType("int");

            builder
                .Property(e => e.Value)
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder
                .Property(e => e.Prob)
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder
                .Property(e => e.Time)
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder
                .HasOne(x => x.DeckBuff)
                .WithMany(x => x.Options)
                .HasForeignKey(x => x.GroupIdX)
                .HasPrincipalKey(x => x.GroupIdX);

            builder
                .HasOne(x => x.DeckBookInfo)
                .WithMany(x => x.Options)
                .HasForeignKey(x => x.OptionId)
                .HasPrincipalKey(x => x.OptionId);

            builder
                .HasData(
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 1,
                        GroupIdX = 1001,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 3,
                        Value = 30,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 2,
                        GroupIdX = 1001,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 3,
                        GroupIdX = 1001,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 4,
                        GroupIdX = 1002,
                        Condition = DeckBuffConditionsEnum.Probability,
                        AtType = DeckBuffAtTypesEnum.NormalAttack,
                        OptionId = 4,
                        Value = 0,
                        Prob = 700,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 5,
                        GroupIdX = 1002,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 6,
                        GroupIdX = 1002,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 7,
                        GroupIdX = 1004,
                        Condition = DeckBuffConditionsEnum.ProbabilityWithDuration,
                        AtType = DeckBuffAtTypesEnum.NormalAttack,
                        OptionId = 1,
                        Value = 15,
                        Prob = 300,
                        Time = 5,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 8,
                        GroupIdX = 1004,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 9,
                        GroupIdX = 1004,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 10,
                        GroupIdX = 1008,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 6,
                        Value = 8,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 11,
                        GroupIdX = 1008,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 12,
                        GroupIdX = 1008,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 13,
                        GroupIdX = 1009,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 6,
                        Value = 15,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 14,
                        GroupIdX = 1009,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 15,
                        GroupIdX = 1009,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 16,
                        GroupIdX = 1010,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 5,
                        Value = 10,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 17,
                        GroupIdX = 1010,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 18,
                        GroupIdX = 1010,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 19,
                        GroupIdX = 1011,
                        Condition = DeckBuffConditionsEnum.ProbabilityWithDuration,
                        AtType = DeckBuffAtTypesEnum.NormalAttack,
                        OptionId = 1,
                        Value = 10,
                        Prob = 600,
                        Time = 10,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 20,
                        GroupIdX = 1011,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 21,
                        GroupIdX = 1011,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 22,
                        GroupIdX = 1012,
                        Condition = DeckBuffConditionsEnum.ProbabilityWithDuration,
                        AtType = DeckBuffAtTypesEnum.SkillAttack,
                        OptionId = 2,
                        Value = 8,
                        Prob = 700,
                        Time = 12,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 23,
                        GroupIdX = 1012,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 24,
                        GroupIdX = 1012,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 25,
                        GroupIdX = 1013,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 3,
                        Value = 15,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 26,
                        GroupIdX = 1013,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 27,
                        GroupIdX = 1013,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 28,
                        GroupIdX = 1016,
                        Condition = DeckBuffConditionsEnum.ProbabilityWithDuration,
                        AtType = DeckBuffAtTypesEnum.SkillAttack,
                        OptionId = 2,
                        Value = 12,
                        Prob = 500,
                        Time = 10,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 29,
                        GroupIdX = 1016,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 30,
                        GroupIdX = 1016,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 31,
                        GroupIdX = 1017,
                        Condition = DeckBuffConditionsEnum.ProbabilityWithDuration,
                        AtType = DeckBuffAtTypesEnum.NormalAttack,
                        OptionId = 1,
                        Value = 12,
                        Prob = 7000,
                        Time = 5,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 32,
                        GroupIdX = 1017,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 5,
                        Value = 15,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 33,
                        GroupIdX = 1017,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 34,
                        GroupIdX = 1019,
                        Condition = DeckBuffConditionsEnum.ProbabilityWithDuration,
                        AtType = DeckBuffAtTypesEnum.NormalAttack,
                        OptionId = 1,
                        Value = 15,
                        Prob = 5000,
                        Time = 5,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 35,
                        GroupIdX = 1019,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 5,
                        Value = 10,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 36,
                        GroupIdX = 1019,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 3,
                        Value = 2,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 37,
                        GroupIdX = 1020,
                        Condition = DeckBuffConditionsEnum.ProbabilityWithDuration,
                        AtType = DeckBuffAtTypesEnum.NormalAttack,
                        OptionId = 1,
                        Value = 25,
                        Prob = 4000,
                        Time = 5,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 38,
                        GroupIdX = 1020,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 5,
                        Value = 15,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 39,
                        GroupIdX = 1020,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 3,
                        Value = 5,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 40,
                        GroupIdX = 1021,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 6,
                        Value = 15,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 41,
                        GroupIdX = 1021,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 5,
                        Value = 15,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 42,
                        GroupIdX = 1021,
                        Condition = DeckBuffConditionsEnum.ProbabilityWithDuration,
                        AtType = DeckBuffAtTypesEnum.NormalAttack,
                        OptionId = 1,
                        Value = 20,
                        Prob = 3000,
                        Time = 7,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 43,
                        GroupIdX = 1022,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 2,
                        Value = 12,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 44,
                        GroupIdX = 1022,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 45,
                        GroupIdX = 1022,
                        Condition = DeckBuffConditionsEnum.None,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 0,
                        Value = 0,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 46,
                        GroupIdX = 1023,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 2,
                        Value = 15,
                        Prob = 0,
                        Time = 0,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 47,
                        GroupIdX = 1023,
                        Condition = DeckBuffConditionsEnum.ProbabilityWithDuration,
                        AtType = DeckBuffAtTypesEnum.NormalAttack,
                        OptionId = 3,
                        Value = 100,
                        Prob = 300,
                        Time = 10,
                    },
                    new DeckBuffOptionAssetDTO
                    {
                        Id = 48,
                        GroupIdX = 1023,
                        Condition = DeckBuffConditionsEnum.Passive,
                        AtType = DeckBuffAtTypesEnum.Passive,
                        OptionId = 6,
                        Value = 15,
                        Prob = 0,
                        Time = 0,
                    }
                );
        }
    }
}
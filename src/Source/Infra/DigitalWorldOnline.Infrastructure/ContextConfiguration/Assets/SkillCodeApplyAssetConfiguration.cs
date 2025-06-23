using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class SkillCodeApplyAssetConfiguration : IEntityTypeConfiguration<SkillCodeApplyAssetDTO>
    {
        public void Configure(EntityTypeBuilder<SkillCodeApplyAssetDTO> builder)
        {
            builder
                .ToTable("SkillCodeApply", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Type)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<SkillCodeApplyTypeEnum, int>(
                    x => (int)x,
                    x => (SkillCodeApplyTypeEnum)x))
                .HasDefaultValue(SkillCodeApplyTypeEnum.Default)
                .IsRequired();
            
            builder
                .Property(x => x.Attribute)
                .HasColumnType("int")
                .HasConversion(new ValueConverter<SkillCodeApplyAttributeEnum, int>(
                    x => (int)x,
                    x => (SkillCodeApplyAttributeEnum)x))
                .HasDefaultValue(SkillCodeApplyAttributeEnum.AllParame)
                .IsRequired();

            builder
               .Property(e => e.Value)
               .HasColumnType("int")
               .IsRequired();

            builder
                .Property(e => e.Chance)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.AdditionalValue)
                .HasColumnType("int");

            builder
               .Property(e => e.IncreaseValue)
               .HasColumnType("int");
        }
    }
}
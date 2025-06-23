using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class SkillInfoAssetConfiguration : IEntityTypeConfiguration<SkillInfoAssetDTO>
    {
        public void Configure(EntityTypeBuilder<SkillInfoAssetDTO> builder)
        {
            builder
                .ToTable("SkillInfo", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.SkillId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();

            builder
               .Property(e => e.FamilyType)
               .HasColumnType("tinyint")
               .IsRequired();

            builder
                .Property(e => e.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(e => e.Description)
                .HasColumnType("varchar")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property(e => e.DSUsage)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.HPUsage)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Value)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.CastingTime)
               .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();


            builder
                .Property(e => e.Cooldown)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.MaxLevel)
                .HasColumnType("tinyint")
                .IsRequired();
            
            builder
                .Property(e => e.RequiredPoints)
                .HasColumnType("tinyint")
                .IsRequired();
            
            builder
                .Property(e => e.Target)
                .HasColumnType("tinyint")
                .IsRequired();
            
            builder
                .Property(e => e.AreaOfEffect)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.AoEMinDamage)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.AoEMaxDamage)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Range)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.UnlockLevel)
                .HasColumnType("tinyint")
                .IsRequired();
            
            builder
                .Property(e => e.MemoryChips)
                .HasColumnType("tinyint")
                .IsRequired();
            
            builder
                .Property(e => e.FirstConditionCode)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.SecondConditionCode)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.ThirdConditionCode)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class MonsterSkillInfoAssetConfiguration : IEntityTypeConfiguration<MonsterSkillInfoAssetDTO>
    {
        public void Configure(EntityTypeBuilder<MonsterSkillInfoAssetDTO> builder)
        {
            builder
                .ToTable("MonsterSkillInfo", "Asset")
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
               .Property(e => e.TargetCount)
               .HasColumnType("tinyint")
               .IsRequired();

            builder
           .Property(e => e.TargetMax)
           .HasColumnType("tinyint")
           .IsRequired();

            builder
           .Property(e => e.TargetMin)
           .HasColumnType("tinyint")
           .IsRequired();


            builder
                .Property(e => e.SkillType)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.RangeId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.MinValue)
                .HasColumnType("int")
                .IsRequired();

            builder
               .Property(e => e.MaxValue)
               .HasColumnType("int")

               .IsRequired();
            builder
             .Property(e => e.CastingTime)
             .HasColumnType("int")
             .IsRequired();


            builder
                .Property(e => e.AnimationDelay)
                 .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();


            builder
              .Property(e => e.NoticeTime)
               .HasColumnType("numeric(9,2)")
                .HasDefaultValue(0)
                .IsRequired();



            builder
                .Property(e => e.Cooldown)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.UseTerms)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.ActiveType)
                .HasColumnType("tinyint")
                .IsRequired();

         
        }
    }
}
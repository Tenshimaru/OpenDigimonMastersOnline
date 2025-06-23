using DigitalWorldOnline.Commons.DTOs.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets
{
    public class BuffAssetConfiguration : IEntityTypeConfiguration<BuffAssetDTO>
    {
        public void Configure(EntityTypeBuilder<BuffAssetDTO> builder)
        {
            builder
                .ToTable("Buff", "Asset")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.BuffId)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.DigimonSkillCode)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.SkillCode)
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.MinLevel)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.ConditionLevel)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.Class)
                .HasColumnType("smallint")
                .IsRequired();
            
            builder
                .Property(e => e.Type)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.LifeType)
                .HasColumnType("int")
                .IsRequired();
            
            builder
                .Property(e => e.TimeType)
                .HasColumnType("int")
                .IsRequired();
        }
    }
}
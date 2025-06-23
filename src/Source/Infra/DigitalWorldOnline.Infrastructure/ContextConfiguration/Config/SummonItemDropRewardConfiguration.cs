using DigitalWorldOnline.Commons.DTOs.Config;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class SummonMobItemDropRewardConfiguration : IEntityTypeConfiguration<SummonMobItemDropDTO>
{
    public void Configure(EntityTypeBuilder<SummonMobItemDropDTO> builder)
    {
        builder
            .ToTable("SummonMobItemDropReward", "Config")
            .HasKey(x => x.Id);

        builder
            .Property(e => e.ItemId)
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();

        builder
            .Property(e => e.MinAmount)
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();

        builder
            .Property(e => e.MaxAmount)
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();

        builder
            .Property(e => e.Chance)
            .HasColumnType("numeric(9,2)")
            .HasDefaultValue(0)
            .IsRequired();



    }
}
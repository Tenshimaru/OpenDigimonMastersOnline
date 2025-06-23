using DigitalWorldOnline.Commons.DTOs.Mechanics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Mechanics
{
    public class GuildConfiguration : IEntityTypeConfiguration<GuildDTO>
    {
        public void Configure(EntityTypeBuilder<GuildDTO> builder)
        {
            builder
                .ToTable("Guild", "Guild")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Level)
                .HasColumnType("tinyint")
                .HasDefaultValue(1)
                .IsRequired();
            
            builder
                .Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(x => x.CreationDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .Property(x => x.CurrentExperience)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(x => x.Notice)
                .HasColumnType("varchar")
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property(x => x.ExtraSlots)
                .HasColumnType("tinyint")
                .HasDefaultValue(byte.MinValue)
                .IsRequired();

            builder
                .HasMany(x => x.Members)
                .WithOne(x => x.Guild)
                .HasForeignKey(x => x.GuildId);
            
            builder
                .HasMany(x => x.Skills)
                .WithOne(x => x.Guild)
                .HasForeignKey(x => x.GuildId);
            
            builder
                .HasMany(x => x.Authority)
                .WithOne(x => x.Guild)
                .HasForeignKey(x => x.GuildId);
            
            builder
                .HasMany(x => x.Historic)
                .WithOne(x => x.Guild)
                .HasForeignKey(x => x.GuildId);
        }
    }
}
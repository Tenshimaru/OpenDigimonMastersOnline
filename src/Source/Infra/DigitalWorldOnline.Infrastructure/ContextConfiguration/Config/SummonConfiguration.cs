using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class SummonConfiguration : IEntityTypeConfiguration<SummonDTO>
    {
        public void Configure(EntityTypeBuilder<SummonDTO> builder)
        {
            builder.ToTable("Summon", "Config")
                .HasKey(x => x.Id);

            builder.Property(e => e.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(e => e.ItemId)
                   .HasColumnName("ItemId")
                   .HasColumnType("int")
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(e => e.Maps)
                   .HasColumnName("Maps")
                   .HasConversion(new IntListToStringConverter())
                   .IsRequired();

            builder.HasMany(x => x.SummonedMobs)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Se houver outras propriedades ou configurações a serem adicionadas, inclua-as aqui
        }
    }
}

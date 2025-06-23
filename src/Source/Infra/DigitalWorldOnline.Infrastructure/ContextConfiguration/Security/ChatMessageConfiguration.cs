using DigitalWorldOnline.Commons.DTOs.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Security
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessageDTO>
    {
        public void Configure(EntityTypeBuilder<ChatMessageDTO> builder)
        {
            builder
                .ToTable("ChatMessage", "Security")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Time)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder
                .Property(x => x.Message)
                .HasColumnType("varchar")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .HasOne(x => x.Character);
        }
    }
}
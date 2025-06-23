using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class CharacterFriendConfiguration : IEntityTypeConfiguration<CharacterFriendDTO>
    {
        public void Configure(EntityTypeBuilder<CharacterFriendDTO> builder)
        {
            builder
                .ToTable("Friend", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(25)
                .IsRequired();

            builder
                .Property(x => x.Annotation)
                .HasColumnType("varchar")
                .HasMaxLength(25)
                .IsRequired();

            builder
                .Property(x => x.Connected)
                .HasColumnType("bit")
                .IsRequired();

            builder
                .Property(x => x.FriendId)
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .HasOne(x => x.Character)
                .WithMany(x => x.Friends)
                .HasForeignKey(x => x.CharacterId);

            builder
                .HasOne(x => x.Friend)
                .WithMany(x => x.Friended)
                .HasForeignKey(x => x.FriendId);
        }
    }
}
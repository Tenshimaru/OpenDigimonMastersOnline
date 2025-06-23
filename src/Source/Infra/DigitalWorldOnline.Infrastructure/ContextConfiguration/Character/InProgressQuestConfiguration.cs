using DigitalWorldOnline.Commons.DTOs.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWorldOnline.Infrastructure.ContextConfiguration.Character
{
    public class InProgressQuestConfiguration : IEntityTypeConfiguration<InProgressQuestDTO>
    {
        public void Configure(EntityTypeBuilder<InProgressQuestDTO> builder)
        {
            builder
                .ToTable("InProgressQuest", "Character")
                .HasKey(x => x.Id);

            builder
                .Property(e => e.QuestId)
                .HasColumnType("smallint")
                .IsRequired();

            builder
                .Property(e => e.FirstCondition)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.SecondCondition)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.ThirdCondition)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.FourthCondition)
                .HasColumnType("tinyint")
                .IsRequired();

            builder
                .Property(e => e.FifthCondition)
                .HasColumnType("tinyint")
                .IsRequired();
        }
    }
}
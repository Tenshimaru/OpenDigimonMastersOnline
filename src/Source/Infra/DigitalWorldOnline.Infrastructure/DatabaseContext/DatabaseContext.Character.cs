using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Event;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Arena;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Character;
using Microsoft.EntityFrameworkCore;

namespace DigitalWorldOnline.Infrastructure
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<CharacterDTO> Character { get; set; }
        public DbSet<CharacterIncubatorDTO> CharacterIncubator { get; set; }
        public DbSet<CharacterMapRegionDTO> CharacterMapRegion { get; set; }
        public DbSet<CharacterSealListDTO> CharacterSealList { get; set; }
        public DbSet<CharacterSealDTO> CharacterSeal { get; set; }
        public DbSet<CharacterXaiDTO> CharacterXai { get; set; }
        public DbSet<CharacterFriendDTO> CharacterFriends { get; set; }
        public DbSet<CharacterArenaPointsDTO> CharacterPoints { get; set; }
        public DbSet<CharacterArenaDailyPointsDTO> CharacterDailyPoints { get; set; }
        public DbSet<CharacterFoeDTO> CharacterFoes { get; set; }
        public DbSet<CharacterTamerSkillDTO> ActiveSkills { get; set; }
        public DbSet<ConsignedShopDTO> CharacterConsignedShop { get; set; }
        public DbSet<CharacterProgressDTO> CharacterProgress { get; set; }
        public DbSet<InProgressQuestDTO> InProgressQuest { get; set; }
        public DbSet<CharacterBuffListDTO> CharacterBuffList { get; set; }
        public DbSet<CharacterLocationDTO> CharacterLocation { get; set; }
        public DbSet<CharacterActiveEvolutionDTO> CharacterActiveEvolution { get; set; }
        public DbSet<CharacterDigimonArchiveDTO> CharacterDigimonArchive { get; set; }
        public DbSet<CharacterDigimonArchiveItemDTO> CharacterDigimonArchiveItem { get; set; }
        public DbSet<CharacterEncyclopediaDTO> CharacterEncyclopedia { get; set; }
        public DbSet<CharacterEncyclopediaEvolutionsDTO> CharacterEncyclopediaEvolutions { get; set; }

        internal static void CharacterEntityConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CharacterConfiguration());
            builder.ApplyConfiguration(new CharacterIncubatorConfiguration());
            builder.ApplyConfiguration(new CharacterXaiConfiguration());
            builder.ApplyConfiguration(new CharacterMapRegionConfiguration());
            builder.ApplyConfiguration(new CharacterSealListConfiguration());
            builder.ApplyConfiguration(new CharacterSealConfiguration());
            builder.ApplyConfiguration(new CharacterFoeConfiguration());
            builder.ApplyConfiguration(new CharacterFriendConfiguration());
            builder.ApplyConfiguration(new CharacterArenaPointsConfiguration());
            builder.ApplyConfiguration(new CharacterArenaDailyPointsConfiguration());
            builder.ApplyConfiguration(new CharacterProgressConfiguration());
            builder.ApplyConfiguration(new InProgressQuestConfiguration());
            builder.ApplyConfiguration(new CharacterBuffListConfiguration());
            builder.ApplyConfiguration(new CharacterBuffConfiguration());
            builder.ApplyConfiguration(new CharacterLocationConfiguration());
            builder.ApplyConfiguration(new CharacterActiveEvolutionConfiguration());
            builder.ApplyConfiguration(new CharacterDigimonArchiveConfiguration());
            builder.ApplyConfiguration(new CharacterDigimonArchiveItemConfiguration());
            builder.ApplyConfiguration(new CharacterTamerSkillConfiguration());
            builder.ApplyConfiguration(new CharacterEncyclopediaConfiguration());
            builder.ApplyConfiguration(new CharacterEncyclopediaEvolutionsConfiguration());
        }
    }
}
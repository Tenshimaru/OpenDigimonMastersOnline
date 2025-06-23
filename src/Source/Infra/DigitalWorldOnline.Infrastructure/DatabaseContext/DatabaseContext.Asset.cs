using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Assets;
using Microsoft.EntityFrameworkCore;

namespace DigitalWorldOnline.Infrastructure
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<MapAssetDTO> MapAsset { get; set; }
        public DbSet<XaiAssetDTO> XaiAsset { get; set; }
        public DbSet<ItemAssetDTO> ItemAsset { get; set; }
        public DbSet<SkillCodeAssetDTO> SkillCodeAsset { get; set; }
        public DbSet<MapRegionListAssetDTO> MapRegionListAsset { get; set; }
        public DbSet<MapRegionAssetDTO> MapRegionAsset { get; set; }
        public DbSet<DigimonBaseInfoAssetDTO> DigimonBaseInfoAsset { get; set; }
        public DbSet<MonsterBaseInfoAssetDTO> MonsterBaseInfoAsset { get; set; }
        public DbSet<DigimonLevelStatusAssetDTO> DigimonLevelStatusAsset { get; set; }
        public DbSet<CharacterBaseStatusAssetDTO> TamerBaseStatusAsset { get; set; }
        public DbSet<CharacterLevelStatusAssetDTO> TamerLevelStatusAsset { get; set; }
        public DbSet<DigimonSkillAssetDTO> DigimonSkillAsset { get; set; }
        public DbSet<MonsterSkillAssetDTO> MonsterSkillAsset { get; set; }
        public DbSet<MonsterSkillInfoAssetDTO> MonsterSkillInfoAsset { get; set; }
        public DbSet<SkillInfoAssetDTO> SkillInfoAsset { get; set; }
        public DbSet<MonthlyEventAssetDTO> MonthlyEvent { get; set; }
        public DbSet<AchievementAssetDTO> AchievementAsset { get; set; }
        public DbSet<CashShopAssetDTO> CashShopAsset { get; set; }
        public DbSet<TimeRewardAssetDTO> TimeRewardAsset { get; set; }
        public DbSet<SealDetailAssetDTO> SealStatusAsset { get; set; }
        public DbSet<EvolutionAssetDTO> EvolutionAssets { get; set; }
        public DbSet<ItemCraftAssetDTO> ItemCraftInfo { get; set; }
        public DbSet<TitleStatusAssetDTO> TitleStatusInfo { get; set; }
        public DbSet<BuffAssetDTO> BuffInfo { get; set; }
        public DbSet<ScanDetailAssetDTO> ScanDetail { get; set; }
        public DbSet<ContainerAssetDTO> Container { get; set; }
        public DbSet<StatusApplyAssetDTO> StatusApply { get; set; }
        public DbSet<AccessoryRollAssetDTO> AccessoryRoll { get; set; }
        public DbSet<PortalAssetDTO> Portals { get; set; }
        public DbSet<QuestAssetDTO> Quests { get; set; }
        public DbSet<HatchAssetDTO> Hatchs { get; set; }
        public DbSet<CloneAssetDTO> Clones { get; set; }
        public DbSet<CloneValueAssetDTO> CloneValues { get; set; }
        public DbSet<NpcAssetDTO> Npcs { get; set; }
        public DbSet<NpcColiseumAssetDTO> NpcColiseum { get; set; }
        public DbSet<TamerSkillAssetDTO> TamerSkills { get; set; }
        public DbSet<ArenaRankingDailyItemRewardsDTO> ArenaDailyItemRewards { get; set; }
        public DbSet<EvolutionArmorAssetDTO> EvolutionsArmor { get; set; }
        public DbSet<ExtraEvolutionNpcAssetDTO> ExtraEvolutionNpc { get; set; }
        public DbSet<GotchaAssetDTO> GotchaAsset { get; set; }
        public DbSet<DeckBookInfoAssetDTO> DeckBookInfo { get; set; }
        public DbSet<DeckBuffAssetDTO> DeckBuff { get; set; }
        public DbSet<DeckBuffOptionAssetDTO> DeckBuffOption { get; set; }

        internal static void AssetsEntityConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new SkillCodeAssetConfiguration());
            builder.ApplyConfiguration(new SkillCodeApplyAssetConfiguration());
            builder.ApplyConfiguration(new MapAssetConfiguration());
            builder.ApplyConfiguration(new DigimonBaseInfoAssetConfiguration());
            builder.ApplyConfiguration(new MonsterBaseInfoAssetConfiguration());
            builder.ApplyConfiguration(new DigimonLevelStatusAssetConfiguration());
            builder.ApplyConfiguration(new CharacterBaseStatusAssetConfiguration());
            builder.ApplyConfiguration(new CharacterLevelStatusAssetConfiguration());
            builder.ApplyConfiguration(new MapRegionListAssetConfiguration());
            builder.ApplyConfiguration(new MapRegionAssetConfiguration());
            builder.ApplyConfiguration(new DigimonSkillAssetConfiguration());
            builder.ApplyConfiguration(new MonsterSkillAssetConfiguration());
            builder.ApplyConfiguration(new MonsterSkillInfoAssetConfiguration());
            builder.ApplyConfiguration(new SkillInfoAssetConfiguration());
            builder.ApplyConfiguration(new SealDetailAssetConfiguration());
            builder.ApplyConfiguration(new EvolutionAssetConfiguration());
            builder.ApplyConfiguration(new EvolutionLineAssetConfiguration());
            builder.ApplyConfiguration(new EvolutionStageAssetConfiguration());
            builder.ApplyConfiguration(new XAIAssetConfiguration());
            builder.ApplyConfiguration(new ItemCraftAssetConfiguration());
            builder.ApplyConfiguration(new ItemCraftMaterialAssetConfiguration());
            builder.ApplyConfiguration(new TitleStatusAssetConfiguration());
            builder.ApplyConfiguration(new BuffAssetConfiguration());
            builder.ApplyConfiguration(new ItemAssetConfiguration());
            builder.ApplyConfiguration(new ScanDetailAssetConfiguration());
            builder.ApplyConfiguration(new ScanRewardDetailAssetConfiguration());
            builder.ApplyConfiguration(new ContainerAssetConfiguration());
            builder.ApplyConfiguration(new ContainerRewardAssetConfiguration());
            builder.ApplyConfiguration(new StatusApplyAssetConfiguration());
            builder.ApplyConfiguration(new AccessoryRollAssetConfiguration());
            builder.ApplyConfiguration(new AccessoryRollStatusAssetConfiguration());
            builder.ApplyConfiguration(new PortalAssetConfiguration());
            builder.ApplyConfiguration(new QuestAssetConfiguration());
            builder.ApplyConfiguration(new QuestConditionsAssetConfiguration());
            builder.ApplyConfiguration(new QuestEventsAssetConfiguration());
            builder.ApplyConfiguration(new QuestGoalsAssetConfiguration());
            builder.ApplyConfiguration(new QuestRewardObjectsAssetConfiguration());
            builder.ApplyConfiguration(new QuestRewardsAssetConfiguration());
            builder.ApplyConfiguration(new QuestSuppliesAssetConfiguration());
            builder.ApplyConfiguration(new HatchAssetConfiguration());
            builder.ApplyConfiguration(new CloneAssetConfiguration());
            builder.ApplyConfiguration(new CloneValueAssetConfiguration());
            builder.ApplyConfiguration(new NpcAssetConfiguration());
            builder.ApplyConfiguration(new NpcItemAssetConfiguration());
            builder.ApplyConfiguration(new NpcPortalsAssetConfiguration());
            builder.ApplyConfiguration(new NpcPortalAssetConfiguration());
            builder.ApplyConfiguration(new NpcPortalsAmountAssetConfiguration());
            builder.ApplyConfiguration(new NpcColiseumAssetConfiguration());
            builder.ApplyConfiguration(new NpcMobInfoAssetConfiguration());
            builder.ApplyConfiguration(new TamerSkillAssetConfiguration());
            builder.ApplyConfiguration(new MonthlyAssetConfiguration());
            builder.ApplyConfiguration(new AchievementAssetConfiguration());
            builder.ApplyConfiguration(new CashShopAssetConfiguration());
            builder.ApplyConfiguration(new TimeRewardAssetConfiguration());
            builder.ApplyConfiguration(new ArenaRankingDailyItemRewardsConfiguration());
            builder.ApplyConfiguration(new ArenaRankingDailyItemRewardConfiguration());
            builder.ApplyConfiguration(new EvolutionArmorAssetConfiguration());
            builder.ApplyConfiguration(new ExtraEvolutionNpcAssetConfiguration());
            builder.ApplyConfiguration(new ExtraEvolutionInformationAssetConfiguration());
            builder.ApplyConfiguration(new ExtraEvolutionAssetConfiguration());
            builder.ApplyConfiguration(new ExtraEvolutionMaterialAssetConfiguration());
            builder.ApplyConfiguration(new ExtraEvolutionRequiredAssetConfiguration());
            builder.ApplyConfiguration(new GotchaAssetConfiguration());
            builder.ApplyConfiguration(new GotchaItemsAssetConfiguration());
            builder.ApplyConfiguration(new GotchaRareItemsAssetConfiguration());
            builder.ApplyConfiguration(new DeckBookInfoConfiguration());
            builder.ApplyConfiguration(new DeckBuffConfiguration());
            builder.ApplyConfiguration(new DeckBuffOptionConfiguration());
        }
    }
}
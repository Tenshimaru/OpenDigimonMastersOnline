using AutoMapper;
using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.Models.Assets;

namespace DigitalWorldOnline.Infrastructure.Mapping
{
    public class AssetsProfile : Profile
    {
        public AssetsProfile()
        {
            CreateMap<MapAssetModel, MapAssetDTO>()
                .ReverseMap();

            CreateMap<StatusAssetModel, StatusDTO>()
                .ReverseMap();

            CreateMap<CharacterBaseStatusAssetModel, CharacterBaseStatusAssetDTO>()
                .ReverseMap();

            CreateMap<CharacterLevelStatusAssetModel, CharacterLevelStatusAssetDTO>()
                .ReverseMap();

            CreateMap<DigimonBaseInfoAssetModel, DigimonBaseInfoAssetDTO>()
                .ReverseMap();

            CreateMap<MonsterBaseInfoAssetModel, MonsterBaseInfoAssetDTO>()
                .ReverseMap();

            CreateMap<DigimonLevelStatusAssetModel, DigimonLevelStatusAssetDTO>()
                .ReverseMap();

            CreateMap<ItemAssetModel, ItemAssetDTO>()
                .ReverseMap();

            CreateMap<SkillCodeAssetModel, SkillCodeAssetDTO>()
                .ReverseMap();

            CreateMap<SkillCodeApplyAssetModel, SkillCodeApplyAssetDTO>()
                .ReverseMap();

            CreateMap<MapRegionListAssetModel, MapRegionListAssetDTO>()
                .ReverseMap();

            CreateMap<MapRegionAssetModel, MapRegionAssetDTO>()
                .ReverseMap();

            CreateMap<DigimonSkillAssetModel, DigimonSkillAssetDTO>()
                .ReverseMap();

            CreateMap<SkillInfoAssetModel, SkillInfoAssetDTO>()
                .ReverseMap();

            CreateMap<MonsterSkillAssetModel, MonsterSkillAssetDTO>()
                .ReverseMap();

            CreateMap<MonsterSkillInfoAssetModel, MonsterSkillInfoAssetDTO>()
                .ReverseMap();

            CreateMap<SealDetailAssetModel, SealDetailAssetDTO>()
                .ReverseMap();

            CreateMap<EvolutionAssetModel, EvolutionAssetDTO>()
                .ReverseMap();

            CreateMap<EvolutionLineAssetModel, EvolutionLineAssetDTO>()
                .ReverseMap();

            CreateMap<EvolutionStageAssetModel, EvolutionStageAssetDTO>()
                .ReverseMap();

            CreateMap<ItemCraftAssetModel, ItemCraftAssetDTO>()
                .ReverseMap();

            CreateMap<ItemCraftMaterialAssetModel, ItemCraftMaterialAssetDTO>()
                .ReverseMap();

            CreateMap<TitleStatusAssetModel, TitleStatusAssetDTO>()
                .ReverseMap();

            CreateMap<BuffInfoAssetModel, BuffAssetDTO>()
                .ReverseMap();

            CreateMap<ScanDetailAssetModel, ScanDetailAssetDTO>()
                .ReverseMap();

            CreateMap<ScanRewardDetailAssetModel, ScanRewardDetailAssetDTO>()
                .ReverseMap();

            CreateMap<StatusApplyAssetModel, StatusApplyAssetDTO>()
                .ReverseMap();

            CreateMap<AccessoryRollAssetModel, AccessoryRollAssetDTO>()
                .ReverseMap();

            CreateMap<AccessoryRollStatusAssetModel, AccessoryRollStatusAssetDTO>()
                .ReverseMap();

            CreateMap<PortalAssetModel, PortalAssetDTO>()
                .ReverseMap();

            CreateMap<ContainerAssetModel, ContainerAssetDTO>()
                .ReverseMap();

            CreateMap<ContainerRewardAssetModel, ContainerRewardAssetDTO>()
                .ReverseMap();

            CreateMap<QuestAssetModel, QuestAssetDTO>()
                .ReverseMap();

            CreateMap<QuestConditionAssetModel, QuestConditionAssetDTO>()
                .ReverseMap();

            CreateMap<QuestEventAssetModel, QuestEventAssetDTO>()
                .ReverseMap();

            CreateMap<QuestGoalAssetModel, QuestGoalAssetDTO>()
                .ReverseMap();

            CreateMap<QuestRewardAssetModel, QuestRewardAssetDTO>()
                .ReverseMap();

            CreateMap<QuestRewardObjectAssetModel, QuestRewardObjectAssetDTO>()
                .ReverseMap();

            CreateMap<QuestSupplyAssetModel, QuestSupplyAssetDTO>()
                .ReverseMap();

            CreateMap<HatchAssetModel, HatchAssetDTO>()
                .ReverseMap();

            CreateMap<CloneAssetModel, CloneAssetDTO>()
                .ReverseMap();

            CreateMap<CloneValueAssetModel, CloneValueAssetDTO>()
                .ReverseMap();

            CreateMap<TamerSkillAssetModel, TamerSkillAssetDTO>()
                .ReverseMap();

            CreateMap<MonthlyEventAssetModel, MonthlyEventAssetDTO>()
                .ReverseMap();

            CreateMap<AchievementAssetModel, AchievementAssetDTO>()
                .ReverseMap();

            CreateMap<CashShopAssetModel, CashShopAssetDTO>()
                .ReverseMap();

            CreateMap<TimeRewardAssetModel, TimeRewardAssetDTO>()
                .ReverseMap();

            CreateMap<NpcAssetModel, NpcAssetDTO>()
                .ReverseMap();

            CreateMap<NpcItemAssetModel, NpcItemAssetDTO>()
                .ReverseMap();

            CreateMap<NpcPortalAssetModel, NpcPortalAssetDTO>()
                .ReverseMap();

            CreateMap<NpcPortalsAmountAssetModel, NpcPortalsAmountAssetDTO>()
                .ReverseMap();

            CreateMap<NpcPortalsAssetModel, NpcPortalsAssetDTO>()
                .ReverseMap();

            CreateMap<NpcColiseumAssetModel, NpcColiseumAssetDTO>()
                .ReverseMap();

            CreateMap<NpcMobInfoAssetModel, NpcMobInfoAssetDTO>()
                .ReverseMap();

            CreateMap<ArenaRankingDailyItemRewardsModel, ArenaRankingDailyItemRewardsDTO>()
                .ReverseMap();

            CreateMap<ArenaRankingDailyItemRewardModel, ArenaRankingDailyItemRewardDTO>()
                .ReverseMap();

            CreateMap<EvolutionArmorAssetModel, EvolutionArmorAssetDTO>()
                .ReverseMap();

            CreateMap<ExtraEvolutionNpcAssetModel, ExtraEvolutionNpcAssetDTO>()
                .ReverseMap();

            CreateMap<ExtraEvolutionInformationAssetModel, ExtraEvolutionInformationAssetDTO>()
                .ReverseMap();

            CreateMap<ExtraEvolutionAssetModel, ExtraEvolutionAssetDTO>()
                .ReverseMap();

            CreateMap<ExtraEvolutionMaterialAssetModel, ExtraEvolutionMaterialAssetDTO>()
                .ReverseMap();

            CreateMap<ExtraEvolutionRequiredAssetModel, ExtraEvolutionRequiredAssetDTO>()
                .ReverseMap();

            CreateMap<XaiAssetModel, XaiAssetDTO>()
                .ReverseMap();

            CreateMap<GotchaAssetModel, GotchaAssetDTO>()
                .ReverseMap();
            CreateMap<GotchaItemsAssetModel, GotchaItemsAssetDTO>()
                .ReverseMap();
            CreateMap<GotchaRareItemsAssetModel, GotchaRareItemsAssetDTO>()
                .ReverseMap();

            CreateMap<DeckBookInfoModel, DeckBookInfoAssetDTO>()
                .ReverseMap();
            CreateMap<DeckBuffModel, DeckBuffAssetDTO>()
                .ReverseMap();
            CreateMap<DeckBuffOptionModel, DeckBuffOptionAssetDTO>()
                .ReverseMap();
        }
    }
}
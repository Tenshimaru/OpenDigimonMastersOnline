using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.DTOs.Chat;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.Server;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Mechanics;
using Microsoft.EntityFrameworkCore;

namespace DigitalWorldOnline.Infrastructure.Repositories.Server
{
    //TODO: separar
    public class ServerQueriesRepository : IServerQueriesRepository
    {
        private readonly DatabaseContext _context;

        public ServerQueriesRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<MapConfigDTO>> GetGameMapsConfigAsync(MapTypeEnum mapType)
        {
            var tamerLocations = await _context.Character
                .AsNoTracking()
                .Where(x => x.EventState == CharacterEventStateEnum.None && x.State == CharacterStateEnum.Loading)
                .Select(x => x.Location)
                .ToListAsync();

            var tamerMapsIds = tamerLocations
                .Select(x => (int)x.MapId)
                .ToList();

            var maps = await _context.MapConfig
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.KillSpawns)
                .ThenInclude(y => y.SourceMobs)
                .Include(x => x.KillSpawns)
                .ThenInclude(y => y.TargetMobs)
                .Include(x => x.Mobs)
                .ThenInclude(y => y.Location)
                .Include(x => x.Mobs)
                .ThenInclude(y => y.ExpReward)
                .Include(x => x.Mobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(z => z.BitsDrop)
                .Include(x => x.Mobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(z => z.Drops)
                .Where(x => x.Type == mapType && tamerMapsIds.Contains(x.MapId))
                .ToListAsync();

            return maps;
        }

        public async Task<DigimonLevelStatusAssetDTO?> GetDigimonLevelingStatusAsync(int type, byte level)
        {
            return await _context.DigimonLevelStatusAsset
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == type && x.Level == level);
        }

        public async Task<DigimonBaseInfoAssetDTO?> GetDigimonBaseInfoAsync(int type)
        {
            return await _context.DigimonBaseInfoAsset
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == type);
        }

        public async Task<IList<DigimonBaseInfoAssetDTO>> GetAllDigimonBaseInfoAsync()
        {
            return await _context.DigimonBaseInfoAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CharacterLevelStatusAssetDTO?> GetTamerLevelingStatusAsync(CharacterModelEnum type,
            byte level)
        {
            return await _context.TamerLevelStatusAsset
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == type && x.Level == level);
        }

        public async Task<CharacterBaseStatusAssetDTO?> GetTamerBaseStatusAsync(CharacterModelEnum type)
        {
            return await _context.TamerBaseStatusAsset
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == type);
        }

        public async Task<ServerDTO?> GetServerByIdAsync(long id)
        {
            return await _context.ServerConfig
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<ServerDTO>> GetServersAsync(AccountAccessLevelEnum accessLevel)
        {
            switch (accessLevel)
            {
                case AccountAccessLevelEnum.Blocked:
                    return new List<ServerDTO>();

                case AccountAccessLevelEnum.Default:
                {
                    return await _context.ServerConfig
                        .AsNoTracking()
                        .Where(x => x.Type == ServerTypeEnum.Default)
                        .ToListAsync();
                }

                default:
                {
                    return await _context.ServerConfig
                        .AsNoTracking()
                        .ToListAsync();
                }
            }
        }

        public async Task<XaiAssetDTO?> GetXaiInformationAsync(int itemId)
        {
            return await _context.XaiAsset
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.ItemId == itemId);
        }

        public async Task<AttendanceRewardDTO?> GetTamerAttendanceAsync(long characterId)
        {
            return await _context.AttendanceReward
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CharacterId == characterId);
        }

        public async Task<List<ItemAssetDTO>> GetItemAssetsAsync()
        {
            return await _context.ItemAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CharacterLevelStatusAssetDTO>> GetTamerLevelAssetsAsync()
        {
            return await _context.TamerLevelStatusAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<DigimonLevelStatusAssetDTO>> GetDigimonLevelAssetsAsync()
        {
            return await _context.DigimonLevelStatusAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<ConsignedShopDTO>> GetConsignedShopsAsync(int mapId)
        {
            return await _context.CharacterConsignedShop
                .AsNoTracking()
                .Where(x => x.Location.MapId == mapId)
                .Include(x => x.Location)
                .ToListAsync();
        }

        public async Task<ConsignedShopDTO?> GetConsignedShopByHandlerAsync(long generalHandler)
        {
            return await _context.CharacterConsignedShop
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GeneralHandler == generalHandler);
        }

        public async Task<ConsignedShopDTO?> GetConsignedShopByTamerIdAsync(long characterId)
        {
            return await _context.CharacterConsignedShop
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CharacterId == characterId);
        }

        public async Task<List<MapAssetDTO>> GetMapAssetsAsync()
        {
            return await _context.MapAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MapRegionListAssetDTO?> GetMapRegionListAssetsAsync(int mapId)
        {
            return await _context.MapRegionListAsset
                .AsNoTracking()
                .Include(x => x.Regions)
                .FirstOrDefaultAsync(x => x.MapId == mapId);
        }

        public async Task<byte> GetCharacterInServerAsync(long accountId, long serverId)
        {
            return (byte)await _context.Character
                .AsNoTracking()
                .CountAsync(x => x.AccountId == accountId && x.ServerId == serverId);
        }

        public async Task<List<DigimonSkillAssetDTO>> GetDigimonSkillAssetsAsync()
        {
            return await _context.DigimonSkillAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SkillCodeAssetDTO>> GetSkillCodeAssetsAsync()
        {
            return await _context.SkillCodeAsset
                .AsNoTracking()
                .Include(x => x.Apply)
                .ToListAsync();
        }

        public async Task<List<SealDetailAssetDTO>> GetSealStatusAssetsAsync()
        {
            return await _context.SealStatusAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<ChatMessageDTO>> GetAllChatMessagesAsync()
        {
            return await _context.ChatMessage
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<EvolutionAssetDTO>> GetDigimonEvolutionAssetsAsync()
        {
            return await _context.EvolutionAssets
                .AsNoTracking()
                .Include(evolution => evolution.Lines)
                .ThenInclude(mainLine => mainLine.Stages)
                .ToListAsync();
        }

        public async Task<EvolutionAssetDTO?> GetDigimonEvolutionAssetsByTypeAsync(int type)
        {
            return await _context.EvolutionAssets
                .AsNoTracking()
                .Include(evoList => evoList.Lines)
                .ThenInclude(mainLine => mainLine.Stages)
                .FirstOrDefaultAsync(x => x.Type == type);
        }

        public async Task<IList<AccountDTO>> GetStaffAccountsAsync()
        {
            return await _context.Account
                .AsNoTracking()
                .Where(x => x.AccessLevel >= AccountAccessLevelEnum.Moderator)
                .ToListAsync();
        }

        public async Task<List<WelcomeMessageConfigDTO>> GetWelcomeMessagesAssetsAsync()
        {
            return await _context.WelcomeMessagesConfig
                .AsNoTracking().ToListAsync();
        }

        public async Task<List<WelcomeMessageConfigDTO>> GetActiveWelcomeMessagesAssetsAsync()
        {
            return await _context.WelcomeMessagesConfig
                .AsNoTracking()
                .Where(x => x.Enabled)
                .ToListAsync();
        }

        public async Task<List<CloneAssetDTO>> GetCloneAssetsAsync()
        {
            return await _context.Clones
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CloneValueAssetDTO>> GetCloneValueAssetsAsync()
        {
            return await _context.CloneValues
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<NpcAssetDTO>> GetNpcAssetsAsync()
        {
            return await _context.Npcs
                .AsNoTracking()
                .Include(x => x.Items)
                .Include(x => x.Portals)
                .ThenInclude(y => y.PortalsAsset)
                .ThenInclude(pa => pa.npcPortalsAsset) // Verifique o nome correto da propriedade
                .ToListAsync();
        }

        public async Task<List<ItemAssetDTO>> GetCloneItemAssetsAsync()
        {
            return await _context.ItemAsset
                .AsNoTracking()
                .Where(x => x.Type == 55 &&
                            x.Name.Contains("Digiclone"))
                .ToListAsync();
        }

        public async Task<ItemCraftAssetDTO?> GetItemCraftAssetsByFilterAsync(int npcId, int seqId)
        {
            return await _context.ItemCraftInfo
                .AsNoTracking()
                .Include(x => x.Materials)
                .FirstOrDefaultAsync(x => x.NpcId == npcId &&
                                          x.SequencialId == seqId);
        }

        public async Task<TitleStatusAssetDTO?> GetTitleStatusAssetsAsync(short titleId)
        {
            return await _context.TitleStatusInfo
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AchievementId == titleId);
        }

        public async Task<List<BuffAssetDTO>> GetBuffInfoAssetsAsync()
        {
            return await _context.BuffInfo
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SkillInfoAssetDTO>> GetSkillInfoAssetsAsync()
        {
            return await _context.SkillInfoAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GuildDTO?> GetGuildByCharacterIdAsync(long characterId)
        {
            return await _context.Guild
                .AsNoTracking()
                .Include(x => x.Authority)
                .Include(x => x.Skills)
                .Include(x => x.Members)
                .Include(x => x.Historic)
                .FirstOrDefaultAsync(x => x.Members.Any(y => y.CharacterId == characterId));
        }

        public async Task<GuildDTO?> GetGuildByGuildNameAsync(string guildName)
        {
            return await _context.Guild
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Name == guildName);
        }

        public async Task<short> GetGuildRankByGuildIdAsync(long guildId)
        {
            return await _context.Guild
                .AsNoTracking()
                .Where(g => g.Id == guildId)
                .Select(g =>
                    (short)(_context.Guild.AsNoTracking()
                        .Count(g2 => (g2.CurrentExperience > g.CurrentExperience) || (g2.Id < g.Id)) + 1))
                .FirstOrDefaultAsync();
        }

        public async Task<GuildDTO?> GetGuildByIdAsync(long guildId)
        {
            return await _context.Guild
                .AsNoTracking()
                .Include(x => x.Authority)
                .Include(x => x.Skills)
                .Include(x => x.Members)
                .Include(x => x.Historic)
                .SingleOrDefaultAsync(x => x.Id == guildId);
        }

        public async Task<UserAccessLevelEnum> CheckPortalAccessAsync(string username, string password)
        {
            var adminUser = await _context.UserConfig
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username &&
                                          x.Password == password);

            return adminUser?.AccessLevel ?? UserAccessLevelEnum.Unauthorized;
        }

        public async Task<UserAccessLevelEnum> GetAdminAccessLevelAsync(string username)
        {
            var dto = await _context.UserConfig
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username);

            return dto?.AccessLevel ?? UserAccessLevelEnum.Unauthorized;
        }

        public async Task<List<UserDTO>> GetAdminUsersAsync()
        {
            return await _context.UserConfig
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<MapConfigDTO>> GetGameMapConfigsForAdminAsync()
        {
            return await _context.MapConfig
                .AsNoTracking()
                .Include(x => x.Mobs)
                .ToListAsync();
        }

        public async Task<List<ScanDetailAssetDTO>> GetScanDetailAssetsAsync()
        {
            return await _context.ScanDetail
                .AsNoTracking()
                .Include(x => x.Rewards)
                .ToListAsync();
        }

        public async Task<List<StatusApplyAssetDTO>> GetStatusApplyInfoAsync()
        {
            return await _context.StatusApply
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TitleStatusAssetDTO>> GetAllTitleStatusInfoAsync()
        {
            return await _context.TitleStatusInfo
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CharacterBaseStatusAssetDTO>> GetAllTamerBaseStatusAsync()
        {
            return await _context.TamerBaseStatusAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<AccessoryRollAssetDTO>> GetAccessoryRollInfoAsync()
        {
            return await _context.AccessoryRoll
                .AsNoTracking()
                .Include(x => x.Status)
                .ToListAsync();
        }

        public async Task<string> GetResourcesHashAsync()
        {
            var dto = await _context.HashConfig
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            return dto?.Hash;
        }

        public async Task<List<PortalAssetDTO>> GetPortalAssetsAsync()
        {
            return await _context.Portals.ToListAsync();
        }

        public async Task<List<ContainerAssetDTO>> GetContainerAssetsAsync()
        {
            return await _context.Container
                .AsNoTracking()
                .Include(x => x.Rewards)
                .ToListAsync();
        }

        public async Task<List<QuestAssetDTO>> GetQuestAssetsAsync()
        {
            return await _context.Quests
                .AsNoTracking()
                .Include(x => x.QuestConditions)
                .Include(x => x.QuestSupplies)
                .Include(x => x.QuestEvents)
                .Include(x => x.QuestGoals)
                .Include(x => x.QuestRewards)
                .ThenInclude(x => x.RewardObjectList)
                .ToListAsync();
        }

        public async Task<DateTime> GetDailyQuestResetTimeAsync()
        {
            var dto = await _context.Routine
                .AsNoTracking()
                .Where(x => x.Type == RoutineTypeEnum.DailyQuests)
                .FirstOrDefaultAsync();

            return dto?.NextRunTime ?? DateTime.MaxValue;
        }

        public async Task<List<HatchAssetDTO>> GetHatchAssetsAsync()
        {
            return await _context.Hatchs
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CloneConfigDTO>> GetCloneConfigsAsync()
        {
            return await _context.CloneConfig
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<HatchConfigDTO>> GetHatchConfigsAsync()
        {
            return await _context.HatchConfig
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<FruitConfigDTO>> GetFruitConfigsAsync()
        {
            return await _context.FruitConfig
                .AsNoTracking()
                .Include(x => x.SizeList)
                .ToListAsync();
        }

        public async Task<List<MonsterSkillAssetDTO>> GetMonsterSkillSkillAssetsAsync()
        {
            return await _context.MonsterSkillAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<MonsterSkillInfoAssetDTO>> GetMonsterSkillInfoAssetsAsync()
        {
            return await _context.MonsterSkillInfoAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TamerSkillAssetDTO>> GetTamerSkillAssetsAsync()
        {
            return await _context.TamerSkills
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<MonthlyEventAssetDTO>> GetMonthlyEventAssetsAsync()
        {
            return await _context.MonthlyEvent
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<AchievementAssetDTO>> GetAchievementAssetsAsync()
        {
            return await _context.AchievementAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CashShopAssetDTO>> GetCashShopAssetsAsync()
        {
            return await _context.CashShopAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TimeRewardAssetDTO>> GetTimeRewardAssetsAsync()
        {
            return await _context.TimeRewardAsset
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TimeRewardDTO>> GetTimeRewardEventsAsync()
        {
            return await _context.TimeReward
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SummonDTO>> GetSummonAssetsAsync()
        {
            return await _context.SummonsConfig
                .AsNoTracking()
                .Include(x => x.SummonedMobs)
                .ThenInclude(y => y.Location)
                .Include(x => x.SummonedMobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(t => t.Drops)
                .Include(x => x.SummonedMobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(t => t.BitsDrop)
                .Include(x => x.SummonedMobs)
                .ThenInclude(y => y.ExpReward)
                .ToListAsync();
        }

        public async Task<List<SummonMobDTO>> GetSummonMobAssetsAsync()
        {
            return await _context.SummonsMobConfig.AsNoTracking()
                .Include(x => x.Location)
                .Include(x => x.DropReward)
                    .ThenInclude(dr => dr.Drops)
                .Include(x => x.DropReward)
                    .ThenInclude(dr => dr.BitsDrop)
                .Include(x => x.ExpReward)
                .ToListAsync();
        }

        public async Task<List<NpcColiseumAssetDTO>> GetNpcColiseumAssetsAsync()
        {
            return await _context.NpcColiseum.AsNoTracking()
                .Include(x => x.MobInfo).ToListAsync();
        }

        public async Task<ArenaRankingDTO> GetArenaRankingAsync(ArenaRankingEnum type)
        {
            var dto = await _context.ArenaRanking
                .AsNoTracking()
                .Include(y => y.Competitors)
                .FirstOrDefaultAsync(x => x.Type == type && x.EndDate >= DateTime.Now);

            return dto;
        }

        public async Task<List<ArenaRankingDailyItemRewardsDTO>> GetArenaRankingDailyItemRewardsAsync()
        {
            return await _context.ArenaDailyItemRewards
                .AsNoTracking()
                .Include(x => x.Rewards).ToListAsync();
        }

        public async Task<List<EvolutionArmorAssetDTO>> GetEvolutionArmorAssetsAsync()
        {
            return await _context.EvolutionsArmor
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ExtraEvolutionNpcAssetDTO>> GetExtraEvolutionNpcAssetAsync()
        {
            var dto = await _context.ExtraEvolutionNpc
                .AsNoTracking()
                .Include(x => x.ExtraEvolutionInformation)
                .ThenInclude(y => y.ExtraEvolution)
                .ThenInclude(z => z.Materials)
                .Include(x => x.ExtraEvolutionInformation)
                .ThenInclude(y => y.ExtraEvolution)
                .ThenInclude(z => z.Requireds)
                .ToListAsync();

            return dto;
        }

        public async Task<List<GotchaAssetDTO>> GetGotchaAssetsAsync()
        {
            return await _context.GotchaAsset
                .AsNoTracking()
                .Include(x => x.Items)
                .Include(x => x.RareItems)
                .ToListAsync();
        }

        public async Task<List<DeckBuffAssetDTO>> GetDeckBuffAssetsAsync()
        {
            return await _context.DeckBuff
                .AsNoTracking()
                .Include(x => x.Options)
                .ThenInclude(x => x.DeckBookInfo)
                .ToListAsync();
        }
    }
}
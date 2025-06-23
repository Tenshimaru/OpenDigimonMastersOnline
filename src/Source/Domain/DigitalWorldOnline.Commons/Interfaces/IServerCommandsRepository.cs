using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Models.Servers;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Events;

namespace DigitalWorldOnline.Commons.Interfaces
{
    public interface IServerCommandsRepository
    {
        Task<ServerDTO?> AddServerAsync(ServerObject server);

        Task UpdateServerAsync(long serverId, string serverName, int experience, bool maintenance);

        Task<ConsignedShopDTO?> AddConsignedShopAsync(ConsignedShop personalShop);

        Task<bool> DeleteServerAsync(long id);

        Task DeleteConsignedShopByHandlerAsync(long generalHandler);

        Task<GuildDTO> AddGuildAsync(GuildModel guild);

        Task UpdateGuildNoticeAsync(long guildId, string newMessage);

        Task UpdateGuildAuthorityAsync(GuildAuthorityModel authority);

        Task DeleteGuildAsync(long guildId);

        Task AddGuildHistoricEntryAsync(long guildId, GuildHistoricModel historicEntry);

        Task AddGuildMemberAsync(long guildId, GuildMemberModel member);

        Task UpdateGuildMemberAuthorityAsync(GuildMemberModel guildMember);

        Task DeleteGuildMemberAsync(long characterId, long guildId);
        Task UpdateArenaRankingAsync(ArenaRankingModel arena);
    }
}
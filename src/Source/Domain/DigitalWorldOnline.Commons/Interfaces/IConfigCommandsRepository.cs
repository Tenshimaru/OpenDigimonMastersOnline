using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Servers;
using DigitalWorldOnline.Commons.Models.TamerShop;

namespace DigitalWorldOnline.Commons.Interfaces
{
    public interface IConfigCommandsRepository
    {
        Task<ServerDTO?> AddServerAsync(ServerObject server);

        Task UpdateServerAsync(long serverId, string serverName, int experience, bool maintenance);

        Task UpdateMapConfigAsync(MapConfigModel mapConfig);

        Task<ConsignedShopDTO?> AddConsignedShopAsync(ConsignedShop personalShop);

        Task<bool> DeleteServerAsync(long id);

        Task DeleteMapConfigAsync(long id);

        Task DeleteMobConfigAsync(long id);

        Task DeleteConsignedShopByHandlerAsync(long generalHandler);

        Task<UserDTO?> AddAdminUserAsync(AdminUserModel user);

        Task UpdateAdminUserAsync(AdminUserModel user);

        Task DeleteAdminUserAsync(long id);

        Task UpdateMobConfigAsync(MobConfigModel mobConfig);
    }
}
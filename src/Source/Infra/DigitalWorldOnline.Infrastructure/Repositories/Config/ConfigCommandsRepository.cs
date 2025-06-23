using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.DTOs.Shop;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Servers;
using DigitalWorldOnline.Commons.Models.TamerShop;
using Microsoft.EntityFrameworkCore;

namespace DigitalWorldOnline.Infrastructure.Repositories.Config
{
    public class ConfigCommandsRepository : IConfigCommandsRepository
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public ConfigCommandsRepository(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDTO?> AddAdminUserAsync(AdminUserModel user)
        {
            var dto = _mapper.Map<UserDTO>(user);

            _context.Add(dto);

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<ConsignedShopDTO?> AddConsignedShopAsync(ConsignedShop personalShop)
        {
            var dto = _mapper.Map<ConsignedShopDTO>(personalShop);

            _context.CharacterConsignedShop.Add(dto);

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<ServerDTO?> AddServerAsync(ServerObject server)
        {
            var dto = _mapper.Map<ServerDTO>(server);

            _context.ServerConfig.Add(dto);

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task DeleteConsignedShopByHandlerAsync(long generalHandler)
        {
            var dto = await _context.CharacterConsignedShop
                .FirstOrDefaultAsync(x => x.GeneralHandler == generalHandler);

            if (dto != null)
            {
                _context.Remove(dto);

                _context.SaveChanges();
            }
        }

        public async Task DeleteMapConfigAsync(long id)
        {
            var dto = await _context.MapConfig.FirstOrDefaultAsync(x => x.Id == id);

            if (dto != null)
            {
                _context.Remove(dto);
                _context.SaveChanges();
            }
        }

        public async Task DeleteMobConfigAsync(long id)
        {
            var dto = await _context.MobConfig
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dto != null)
            {
                var mapDto = await _context.MapConfig
                    .Include(x => x.Mobs)
                    .FirstOrDefaultAsync(x => x.Id == dto.GameMapConfigId);

                if (mapDto != null)
                {
                    mapDto.Mobs?.Remove(dto);
                    _context.Update(mapDto);
                }

                _context.Remove(dto);
                _context.SaveChanges();
            }
        }

        public async Task<bool> DeleteServerAsync(long id)
        {
            var dto = await _context.ServerConfig.FirstOrDefaultAsync(x => x.Id == id);

            if (dto != null)
            {
                var chars = _context.Character.Count(x => x.ServerId == id);

                if (chars == default)
                {
                    _context.Remove(dto);
                    _context.SaveChanges();
                }
                else
                    return false;
            }

            return true;
        }

        public async Task UpdateAdminUserAsync(AdminUserModel user)
        {
            var dto = await _context.UserConfig
                .FirstOrDefaultAsync(x => x.Id == user.Id);

            if (dto != null)
            {
                dto.AccessLevel = user.AccessLevel;
                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task DeleteAdminUserAsync(long id)
        {
            var dto = await _context.UserConfig
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dto != null)
            {
                _context.Remove(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdateMapConfigAsync(MapConfigModel mapConfig)
        {
            var dto = await _context.MapConfig.FirstOrDefaultAsync(x => x.Id == mapConfig.Id);

            if (dto != null)
            {
                dto.MapId = mapConfig.MapId;
                dto.Name = mapConfig.Name;

                dto.Mobs = _mapper.Map<List<MobConfigDTO>>(mapConfig.Mobs);

                _context.Update(dto);

                _context.SaveChanges();
            }
        }

        public async Task UpdateMobConfigAsync(MobConfigModel mobConfig)
        {
            var dto = await _context.MobConfig
                .AsNoTracking()
                .Include(x => x.ExpReward)
                .Include(x => x.DropReward)
                .Include(x => x.Location)
                .FirstOrDefaultAsync(x => x.Id == mobConfig.Id);

            if (dto != null)
            {
                var gameMapConfigId = dto.GameMapConfigId;
                dto = _mapper.Map<MobConfigDTO>(mobConfig);
                dto.GameMapConfigId = gameMapConfigId;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdateServerAsync(long serverId, string serverName, int experience, bool maintenance)
        {
            var dto = await _context.ServerConfig.FirstOrDefaultAsync(x => x.Id == serverId);

            if (dto != null)
            {
                dto.Name = serverName;
                dto.Experience = experience;
                dto.Maintenance = maintenance;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }
    }
}
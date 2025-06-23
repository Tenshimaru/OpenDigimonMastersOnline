using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalWorldOnline.Infrastructure.Repositories.Config
{
    public class ConfigQueriesRepository : IConfigQueriesRepository
    {
        private readonly DatabaseContext _context;

        public ConfigQueriesRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IList<MobConfigDTO>> GetMapMobsConfigAsync(long configId)
        {
            return await _context.MobConfig
                .AsNoTracking()
                .AsSplitQuery()
                .Include(y => y.Location)
                .Include(y => y.ExpReward)
                .Include(y => y.DropReward)
                .ThenInclude(z => z.Drops)
                .Include(y => y.DropReward)
                .ThenInclude(z => z.BitsDrop)
                .Where(x => x.GameMapConfigId == configId)
                .ToListAsync();
        }

        public async Task<IList<MapConfigDTO>> GetGameMapConfigsAsync()
        {
            return await _context.MapConfig
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.Mobs)
                .ThenInclude(y => y.Location)
                .Include(x => x.Mobs)
                .ThenInclude(y => y.ExpReward)
                .Include(x => x.Mobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(z => z.Drops)
                .Include(x => x.Mobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(z => z.BitsDrop)
                .ToListAsync();
        }

        public async Task<MapConfigDTO?> GetGameMapConfigByMapIdAsync(int mapId)
        {
            return await _context.MapConfig
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MapId == mapId);
        }

        public async Task<MapConfigDTO?> GetGameMapConfigByIdAsync(long id)
        {
            return await _context.MapConfig
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<MobConfigDTO>> GetMapMobsByIdAsync(int mapId)
        {
            var mapDto = await _context.MapConfig.FirstOrDefaultAsync(x => x.MapId == mapId);

            if (mapDto != null)
            {
                return await _context.MobConfig
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(y => y.Location)
                    .Include(y => y.ExpReward)
                    .Include(y => y.DropReward)
                    .ThenInclude(z => z.Drops)
                    .Include(y => y.DropReward)
                    .ThenInclude(z => z.BitsDrop)
                    .Where(x => x.GameMapConfigId == mapDto.Id)
                    .ToListAsync();
            }
            else
                return default;
        }

        public async Task<List<EventConfigDTO>> GetEventsConfigAsync()
        {
            return await _context.EventConfig
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Map)
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Mobs)
                .ThenInclude(y => y.Location)
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Mobs)
                .ThenInclude(y => y.ExpReward)
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Mobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(z => z.Drops)
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Mobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(z => z.BitsDrop)
                .ToListAsync();
        }

        public async Task<List<EventConfigDTO>> GetEventsConfigAsync(bool isEnabled)
        {
            return await _context.EventConfig
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Map)
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Mobs)
                .ThenInclude(y => y.Location)
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Mobs)
                .ThenInclude(y => y.ExpReward)
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Mobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(z => z.Drops)
                .Include(x => x.EventMaps)
                .ThenInclude(y => y.Mobs)
                .ThenInclude(y => y.DropReward)
                .ThenInclude(z => z.BitsDrop)
                .Where(x => x.IsEnabled == isEnabled)
                .ToListAsync();
        }
    }
}
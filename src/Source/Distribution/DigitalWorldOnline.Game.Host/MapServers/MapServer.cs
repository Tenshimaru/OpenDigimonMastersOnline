using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class MapServer
    {
        private readonly PartyManager _partyManager;
        private readonly StatusManager _statusManager;
        private readonly ExpManager _expManager;
        private readonly DropManager _dropManager;
        private readonly AssetsLoader _assets;
        private readonly ConfigsLoader _configs;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventManager _eventManager;

        public List<GameMap> Maps { get; set; }

        public MapServer(
            PartyManager partyManager,
            AssetsLoader assets,
            ConfigsLoader configs,
            StatusManager statusManager,
            ExpManager expManager,
            DropManager dropManager,
            ILogger logger,
            ISender sender,
            IMapper mapper,
            IServiceProvider serviceProvider,
            EventManager eventManager)
        {
            _partyManager = partyManager;
            _statusManager = statusManager;
            _expManager = expManager;
            _dropManager = dropManager;
            _assets = assets.Load();
            _configs = configs.Load();
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _eventManager = eventManager;

            Maps = new List<GameMap>();
        }

        private void SaveMobToDatabase(MobConfigModel mob)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var mobDto = dbContext.MobConfig.SingleOrDefault(m => m.Id == mob.Id);

                if (mobDto == null)
                {
                    _logger.Error($"BOSS {mob.Name},{mob.Id} Does not exist in the database Unable to call MobConfig.");
                    return;
                }

                mobDto.DeathTime = mob.DeathTime;
                mobDto.ResurrectionTime = mob.ResurrectionTime;

                try
                {
                    dbContext.SaveChanges();
                    _logger.Information($"BOSS {mob.Name},{mob.Id} Update seuccess.");
                }
                catch (Exception ex)
                {
                    _logger.Error($"BOSS time Update error： {mob.Name} (Id: {mob.Id}): {ex.Message}");
                }
            }
        }

    }
}
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Game.Managers;
using AutoMapper;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class PvpServer
    {
        private readonly StatusManager _statusManager;
        private readonly DropManager _dropManager;
        private readonly PartyManager _partyManager;
        private readonly AssetsLoader _assets;
        private readonly ConfigsLoader _configs;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public List<GameMap> Maps { get; set; }

        public PvpServer(StatusManager statusManager, DropManager dropManager, PartyManager partyManager, AssetsLoader assets, ConfigsLoader configs, ILogger logger, ISender sender, IMapper mapper, IServiceProvider serviceProvider)
        {
            _statusManager = statusManager;
            _dropManager = dropManager;
            _partyManager = partyManager;
            _assets = assets.Load();
            _configs = configs.Load();
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
            _serviceProvider = serviceProvider;

            Maps = new List<GameMap>();
        }
    }
}
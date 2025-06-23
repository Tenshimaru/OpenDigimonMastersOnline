using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Game.Managers;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class DungeonsServer
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

        public List<GameMap> Maps { get; set; }

        public DungeonsServer(PartyManager partyManager, StatusManager statusManager, ExpManager expManager, DropManager dropManager,
           AssetsLoader assets, ConfigsLoader configs, ILogger logger, ISender sender, IMapper mapper)
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

            Maps = new List<GameMap>();
        }
    }
}
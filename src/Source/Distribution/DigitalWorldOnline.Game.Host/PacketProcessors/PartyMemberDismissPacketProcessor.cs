using Microsoft.Extensions.Configuration;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartyMemberDismissPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartyMemberDismiss;

        private const string GameServerAddress = "GameServer:Address";
        private const string GamerServerPublic = "GameServer:PublicAddress";
        private const string GameServerPort = "GameServer:Port";

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IConfiguration _configuration;

        public PartyMemberDismissPacketProcessor(PartyManager partyManager, MapServer mapServer, DungeonsServer dungeonServer,
            ILogger logger, ISender sender, IConfiguration configuration)
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _logger = logger;
            _sender = sender;
            _configuration = configuration;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            _logger.Information($"Party Dismiss Packet !!");

            var packet = new GamePacketReader(packetData);

            //var party = _partyManager.FindParty(client.TamerId);

        }
    }
}

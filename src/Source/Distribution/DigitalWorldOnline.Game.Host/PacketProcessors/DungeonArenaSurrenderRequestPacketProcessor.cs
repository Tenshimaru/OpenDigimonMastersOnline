using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class DungeonArenaSurrenderPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.DungeonArenaSurrender;

        private readonly DungeonsServer _dungeonServer;
        private readonly IConfiguration _configuration;
        private readonly ISender _sender;
        private readonly ILogger _logger;
        private readonly PartyManager _partyManager;
        private const string GameServerAddress = "GameServer:Address";
        private const string GamerServerPublic = "GameServer:PublicAddress";
        private const string GameServerPort = "GameServer:Port";

        public DungeonArenaSurrenderPacketProcessor(
           DungeonsServer dungeonServer,
            IConfiguration configuration,
            ISender sender,
            ILogger logger,
            PartyManager partyManager)
        {
            _configuration = configuration;
            _dungeonServer = dungeonServer;
            _sender = sender;
            _logger = logger;
            _partyManager = partyManager;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(3));
            var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(3));

            if (mapConfig == null || waypoints == null || !waypoints.Regions.Any())
            {
                client.Send(new SystemMessagePacket($"Map information not found for map Id {mapConfig?.MapId}."));
                _logger.Warning($"Map information not found for map Id {3} on character {client.TamerId} jump booster.");
                return;
            }

            var party = _partyManager.FindParty(client.TamerId);

            var destination = waypoints.Regions.First();

            if (party != null)
            {
                foreach (var member in party.Members.Values.Where(x=> x.Id != client.TamerId))
                {
                    var targetClient = _dungeonServer.FindClientByTamerId(member.Id);

                    if(targetClient != null)
                    {
                        _dungeonServer.RemoveClient(targetClient);

                        targetClient.Tamer.NewLocation(3, destination.X, destination.Y);
                        await _sender.Send(new UpdateCharacterLocationCommand(targetClient.Tamer.Location));

                        client.Tamer.Partner.NewLocation(3, destination.X, destination.Y);
                        await _sender.Send(new UpdateDigimonLocationCommand(targetClient.Tamer.Partner.Location));

                        targetClient.Tamer.UpdateState(CharacterStateEnum.Loading);
                        await _sender.Send(new UpdateCharacterStateCommand(targetClient.TamerId, CharacterStateEnum.Loading));

                        targetClient.SetGameQuit(false);

                        targetClient.Send(new MapSwapPacket(
                            _configuration[GamerServerPublic],
                            _configuration[GameServerPort],
                           targetClient.Tamer.Location.MapId,
                           targetClient.Tamer.Location.X,
                           targetClient.Tamer.Location.Y).Serialize());

                    }
                }
            }

            _dungeonServer.RemoveClient(client);

            client.Tamer.NewLocation(3, destination.X, destination.Y);
            await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

            client.Tamer.Partner.NewLocation(3, destination.X, destination.Y);
            await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));

            client.Tamer.UpdateState(CharacterStateEnum.Loading);
            await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

            client.SetGameQuit(false);

            client.Send(new MapSwapPacket(
                _configuration[GamerServerPublic],
                _configuration[GameServerPort],
                client.Tamer.Location.MapId,
                client.Tamer.Location.X,
                client.Tamer.Location.Y).Serialize());
        }
    }
}